using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class GridPositionData
{
    public Vector2Int gridCell;
    public Vector3 gridPos;
}

public class TileGridGenerator : MonoBehaviour
{
    public TileGridState currentTileGridState;

    public GameObject tileLayerPrefab;
    public LayerMask tileLayer;
 
    public float tileSpacing = 0.1f;
    public float layerSpacing = -0.25f;

    [Header("Tween settings")]
    float baseDelay = 0.05f;
    float baseWaveSpeed = 0.05f;

    int iceTileCount = 1;
    int maxIceTileCount = 3;

    private List<List<GridPositionData>> gridPositions = new List<List<GridPositionData>>();
    public List<KeyValuePair<GameObject, List<Tile>>> tiles = new List<KeyValuePair<GameObject, List<Tile>>>();

    public void GenerateGridPositions(int layer, int layerRows, int layerColumns, GameObject layerRendererObj)
    {
        SpriteRenderer spriteRenderer = layerRendererObj.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on layerRendererObj.");
            return;
        }

        // Size of the SpriteRenderer
        Vector2 spriteSize = spriteRenderer.size;
        float spriteWidth = spriteSize.x;
        float spriteHeight = spriteSize.y;

        float totalGridWidth = (layerColumns - 1) * (1 + tileSpacing);
        float totalGridHeight = (layerRows - 1) * (1 + tileSpacing);

        if (totalGridWidth > spriteWidth || totalGridHeight > spriteHeight)
        {
            Debug.LogWarning("Grid exceeds the bounds of the SpriteRenderer. Adjust rows/columns or spacing.");
        }

        // Starting position to center the grid within the SpriteRenderer bounds
        float startPosX = spriteRenderer.transform.position.x - (spriteWidth / 2f) + (spriteWidth - totalGridWidth) / 2f;
        float startPosY = spriteRenderer.transform.position.y + (spriteHeight / 2f) - (spriteHeight - totalGridHeight) / 2f;

        List<GridPositionData> currentLayerGridPositions = new List<GridPositionData>();

        for (int y = 0; y < layerRows; y++)
        {
            for (int x = 0; x < layerColumns; x++)
            {
                Vector3 gridPosition = new Vector3(
                    startPosX + x * (1 + tileSpacing),
                    startPosY - y * (1 + tileSpacing),
                    layer * layerSpacing
                );

                GridPositionData gridPositionData = new GridPositionData();
                gridPositionData.gridCell = new Vector2Int(x, y);
                gridPositionData.gridPos = gridPosition;

                currentLayerGridPositions.Add(gridPositionData);
            }
        }

        gridPositions.Add(currentLayerGridPositions);
    }

    public void GenerateCustomTileGrid(CustomLevelData customLevelData, LevelData currentLevelData)
    {
        currentLevelData.customGridCells = customLevelData.customGridCells;

        GenerateTileGrid(currentLevelData, true);
    }

    public void GenerateTileGrid(LevelData levelData, bool isCustomLevel = false, bool isAdjustmentLayer = false)
    {
        currentTileGridState = TileGridState.Generating;

        gridPositions.Clear();

        int levelRows = levelData.rows;
        int levelColumns = levelData.columns;
        int levelLayers = levelData.layers;

        int previousLayerRow = levelRows;
        int previousLayerColumn = levelColumns;

        bool skipCellsInLayer = false;

        HashSet<Vector2Int> currentLayerCells = new HashSet<Vector2Int>(levelData.customGridCells);

        if (isCustomLevel)
        {
            (levelRows, levelColumns) = Utils.GetGridDimensions(levelData.customGridCells);
            levelLayers = 1;
        }

        if (levelRows == 0 && levelColumns == 0) return;

        for (int layer = 0; layer <= levelLayers; layer++)
        {
            if (isCustomLevel)
            {
                if (layer > 0)
                {
                    if (currentLayerCells.Count <= 0) return;
                }
            }

            List<Tile> layerTiles = new List<Tile>();

            int layerRows = (levelData.reducedRows) ? levelRows - layer : levelRows;
            int layerColumns = (levelData.reducedColumns) ? levelColumns - layer : levelColumns;

            iceTileCount = 1;
            maxIceTileCount = 0;

            // Increase layer size randomly and skip this for adjustment layer to not mess with validation
            if (!isAdjustmentLayer)
            {
                layerRows = levelData.increasedRows ? Mathf.Min(previousLayerRow + 1, GameManager.Instance.levelManager.MAX_ROWS)
                            : layerRows;

                layerColumns = levelData.increasedColumns ? Mathf.Min(previousLayerColumn + 1, GameManager.Instance.levelManager.MAX_COLUMNS)
                               : layerColumns;

                skipCellsInLayer = Utils.GetRandomBool(0.5f);

                maxIceTileCount = 3;
            }

            int tileRendererLayer = tiles.Count;
            GameObject layerRendererObj = CreateTileRendererLayer(tileRendererLayer);

            GenerateGridPositions(layer, layerRows, layerColumns, layerRendererObj);

            for (int y = 0; y < layerRows; y++)
            {
                for (int x = 0; x < layerColumns; x++)
                {
                    var cell = new Vector2Int(x, y);

                    if (isCustomLevel)
                    {
                        if (!currentLayerCells.Contains(cell)) continue;
                    }
                    else if (!isCustomLevel && skipCellsInLayer)
                    {
                        bool skipCell = Utils.GetRandomBool(0.35f);
                        if (skipCell) continue;
                    }
                        
                    Vector3 tilePosition = GetLayerGridPosFromCell(layer, cell);

                    if (tilePosition == Vector3.positiveInfinity)
                    {
                        Debug.Log($"Tile position not found for grid cell {cell}");
                        continue;
                    }

                    tilePosition.z = tileRendererLayer * layerSpacing;
                    var tile = CreateTile(levelData, tilePosition, layerRendererObj, tileRendererLayer, cell);
                    layerTiles.Add(tile);
                }
            }

            previousLayerRow = layerRows;
            previousLayerColumn = layerColumns;

            currentLayerCells = Utils.GetInnerLayerCells(currentLayerCells);

            tiles.Add(new KeyValuePair<GameObject, List<Tile>>(layerRendererObj, layerTiles));
        }
    }

    private Tile CreateTile(LevelData levelData, Vector3 tilePos, GameObject layerRendererObj, int layer, Vector2Int cell)
    {
        GameObject tileObj = Instantiate(levelData.tilePrefab, tilePos, Quaternion.identity, layerRendererObj.transform);
        tileObj.TryGetComponent<Tile>(out Tile tile);
        tile.tileLayer = layer;

        tileObj.SetActive(false);
        tileObj.name = $"Tile_{cell.x}_{cell.y}";

        if (levelData.hasIceTiles)
        {
            bool isIceTile = Utils.GetRandomBool();

            if (isIceTile)
            {
                if (iceTileCount <= maxIceTileCount)
                {
                    tile.tileIceManager.EnableIce();
                    iceTileCount++;
                }
            }
        }

        return tile;
    }

    private Vector3 GetLayerGridPosFromCell(int layer, Vector2Int gridCell)
    {
        var layerGridPositions = gridPositions[layer];

        foreach (var gridData in layerGridPositions)
        {
            if (gridData.gridCell == gridCell)
            {
                return gridData.gridPos;
            }
        }

        return Vector3.positiveInfinity;
    }

    private void UpdateTilesStatus(int layer)
    {
        if (layer < 0) return;

        var layerTiles = tiles[layer].Value;

        foreach (var tile in layerTiles)
        {
            Vector3 tileBoxPos = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z + -0.16f);
            Vector3 tileBoxScale = new Vector3(tile.transform.localScale.x, tile.transform.localScale.y, tile.transform.localScale.z * (Mathf.Abs(layerSpacing) + 0.1f)) * 0.85f;
            Collider[] colliders = Physics.OverlapBox(tileBoxPos, tileBoxScale / 2, Quaternion.identity, tileLayer);

            foreach (Collider collider in colliders)
            {
                if (collider.gameObject == tile.gameObject) continue;

                tile.SetTileState(false);
                //Debug.Log(tile.gameObject.name + " has hit : " + collider.name);
            }

            if (colliders.Length <= 0)
            {
                tile.SetTileState(true);
            }
        }
    }

    private void UpdateIceTilesStatus()
    {
        foreach (var tileValuePair in tiles)
        {
            var layerTiles = tileValuePair.Value;

            foreach (var tile in layerTiles)
            {
                if (tile.State)
                {
                    if (tile.tileIceManager.isIceTile)
                    {
                        tile.tileIceManager.BreakIce();
                    }
                }
            }
        }
    }

    private void CheckIsLevelDone()
    {
        if (GetTotalTileCount() <= 0)
        {
            UIManager.Instance.onLevelDoneEvent.Invoke();
            Debug.Log("Level done");
        }
    }

    public void OnTileRemoved(Tile removedTile)
    {
        int layerBelowTile = removedTile.tileLayer - 1;

        tiles[removedTile.tileLayer].Value.Remove(removedTile);

        UpdateIceTilesStatus();
        UpdateTilesStatus(layerBelowTile);

        CheckIsLevelDone();
    }

    public void OnTileUndo(Tile undoTile)
    {
        int layerBelowTile = undoTile.tileLayer - 1;

        tiles[undoTile.tileLayer].Value.Add(undoTile);
        UpdateTilesStatus(layerBelowTile);
    }

    public void GenerateScrews()
    {
        List<Color> screwColors = GenerateScrewColorGroups();
        int colorIndex = 0;

        foreach (var layerTiles in tiles)
        {
            var tileList = layerTiles.Value;

            foreach (var tile in tileList)
            {
                tile.screw.SetColor(screwColors[colorIndex]);
                colorIndex++;
            }
        }
    }

    private List<Color> GenerateScrewColorGroups()
    {
        var colorData = GameManager.Instance.levelManager.colorData;
        List<Color> screwColors = new List<Color>();
        int totalTiles = GetTotalTileCount();
        int tileGroups = totalTiles / 3;

        List<Color> availableColors = new List<Color>();
        foreach (var colorEntry in colorData)
        {
            availableColors.Add(colorEntry.color);
        }

        Utils.ShuffleList(ref availableColors);
        Color lastColor = Color.clear;

        for (int i = 0; i < tileGroups; i++)
        {
            if (availableColors.Count == 0)
            {
                foreach (var colorEntry in colorData)
                {
                    availableColors.Add(colorEntry.color);
                }
                Utils.ShuffleList(ref availableColors);
            }

            Color groupColor = availableColors[0];
            availableColors.RemoveAt(0);

            if (groupColor == lastColor && availableColors.Count > 0)
            {
                availableColors.Add(groupColor);
                groupColor = availableColors[0];
                availableColors.RemoveAt(0);
            }

            for (int j = 0; j < 3; j++)
            {
                screwColors.Add(groupColor);
            }

            lastColor = groupColor;
        }

        Utils.ShuffleList(ref screwColors);

        return screwColors;
    }

    private GameObject CreateTileRendererLayer(int layerNum)
    {
        GameObject tileLayerObj = Instantiate(tileLayerPrefab, transform);
        tileLayerObj.name = "Layer " + layerNum;

        tileLayerObj.GetComponent<SortingGroup>().sortingOrder = layerNum;

        return tileLayerObj;
    }

    public int GetTotalTileCount()
    {
        int totalTiles = 0;

        foreach (var tilesValuePair in tiles)
        {
            totalTiles += tilesValuePair.Value.Count;
        }

        return totalTiles;
    }

    public IEnumerator TileGridTweenAnimate()
    {
        for (int layer = 0; layer < tiles.Count; layer++)
        {
            Sequence tileLayerWaveSequence = DOTween.Sequence();
            float waveSpeed = baseWaveSpeed + (layer * 0.05f);

            var layerTiles = tiles[layer].Value;

            foreach (var tile in layerTiles)
            {
                tile.gameObject.SetActive(true);

                float delay = baseDelay * waveSpeed;
                Vector3 startScale = Vector3.zero;
                Vector3 endScale = tile.transform.localScale;

                tile.transform.localScale = startScale;
                Tween tileScaleTween = tile.transform.DOScale(endScale, 0.5f)
                                        .SetDelay(delay)
                                        .SetEase(Ease.OutBack);

                tileLayerWaveSequence.Join(tileScaleTween);
            }

            yield return tileLayerWaveSequence.WaitForCompletion();

            UpdateTilesStatus(layer - 1);
        }

        currentTileGridState = TileGridState.Done;
    }

    public bool ClearGrid()
    {
        foreach (var tilesValuePair in tiles)
        {
            var layerTiles = tilesValuePair.Value;

            foreach (var tile in layerTiles)
            {
                Destroy(tile.gameObject);
            }

            Destroy(tilesValuePair.Key);
        }

        tiles.Clear();

        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (!Application.isPlaying) return;

        foreach (var tilesValuePair in tiles)
        {
            var tiles = tilesValuePair.Value;

            foreach (var tile in tiles)
            {
                Vector3 tileBoxPos = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z + -0.16f);
                Vector3 tileBoxScale = new Vector3(tile.transform.localScale.x, tile.transform.localScale.y, tile.transform.localScale.z * (Mathf.Abs(layerSpacing) + 0.1f)) * 0.85f;
                Gizmos.DrawWireCube(tileBoxPos, tileBoxScale);
            }
        }

        for (int layer = 0; layer < gridPositions.Count; layer++)
        {
            var layerGridPositions = gridPositions[layer];

            foreach (var gridPos in layerGridPositions)
            {
                //Gizmos.DrawSphere(gridPos.gridPos, 0.15f);
            }
        }
    }
}