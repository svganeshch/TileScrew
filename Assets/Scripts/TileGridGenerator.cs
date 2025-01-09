using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TileGridGenerator : MonoBehaviour
{
    public GameObject tileLayerPrefab;
    public LayerMask tileLayer;
 
    public float tileSpacing = 0.1f;
    public float layerSpacing = -0.25f;

    private Dictionary<int, List<Tile>> tiles = new Dictionary<int, List<Tile>>();

    public void GenerateGrid(LevelData levelData)
    {
        int layer = Mathf.Max(tiles.Count, 0);
        float levelRows = levelData.rows;
        float levelColumns = levelData.columns;

        GameObject layerRendererObj = CreateTileRendererLayer(layer);
        float layerRows = (levelData.reducedRows) ? levelRows - layer : levelRows;
        float layerColumns = (levelData.reducedColumns) ? levelColumns - layer : levelColumns;

        float totalWidth = (layerColumns - 1) * (1 + tileSpacing);
        float totalHeight = (layerRows - 1) * (1 + tileSpacing);

        float startPosX = -totalWidth / 2f;
        float startPosY = totalHeight / 2f;

        List<Tile> layerTiles = new List<Tile>();

        for (int y = 0; y < layerRows; y++)
        {
            for (int x = 0; x < layerColumns; x++)
            {
                Vector3 tilePosition = new Vector3(
                    startPosX + x * (1 + tileSpacing),
                    startPosY - y * (1 + tileSpacing),
                    layer * layerSpacing
                );

                GameObject tileObj = Instantiate(levelData.tilePrefab, tilePosition, Quaternion.identity, layerRendererObj.transform);

                tileObj.TryGetComponent<Tile>(out Tile tile);
                tile.tileLayer = layer;
                layerTiles.Add(tile);

                tileObj.name = $"Tile_{x}_{y}";
            }
        }

        tiles.Add(layer, layerTiles);

        UpdateTilesStatus(layer - 1);
    }

    private void UpdateTilesStatus(int layer)
    {
        if (layer < 0) return;

        var layerTiles = tiles[layer];

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

    public void OnTileRemoved(Tile removedTile)
    {
        int layerBelowTile = removedTile.tileLayer - 1;

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

        for (int i = 0; i < tileGroups; i++)
        {
            Color color = colorData[Random.Range(0, colorData.Length)].color;

            for (int j = 0; j < 3; j++)
            {
                screwColors.Add(color);
            }
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

        foreach (var layerTiles in tiles)
        {
            totalTiles += layerTiles.Value.Count;
        }

        return totalTiles;
    }

    public void ClearGrid()
    {
        foreach (var layer in tiles)
        {
            var layerTiles = layer.Value;

            foreach (var tile in layerTiles)
            {
                Destroy(tile.gameObject);
            }
        }

        tiles.Clear();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (!Application.isPlaying) return;

        foreach (var layerTiles in tiles)
        {
            var tiles = layerTiles.Value;

            foreach (var tile in tiles)
            {
                Vector3 tileBoxPos = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z + -0.16f);
                Vector3 tileBoxScale = new Vector3(tile.transform.localScale.x, tile.transform.localScale.y, tile.transform.localScale.z * (Mathf.Abs(layerSpacing) + 0.1f)) * 0.85f;
                Gizmos.DrawWireCube(tileBoxPos, tileBoxScale);
            }
        }
    }
}