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
        float levelRows = levelData.rows;
        float levelColumns = levelData.columns;

        for (int i = 0; i <= levelData.layers; i++)
        {
            GameObject layerRendererObj = CreateTileRendererLayer(i);
            float layerRows = (levelData.reducedRows) ? levelRows - i : levelRows;
            float layerColumns = (levelData.reducedColumns) ? levelColumns - i : levelColumns;

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
                        i * layerSpacing
                    );

                    GameObject tileObj = Instantiate(levelData.tilePrefab, tilePosition, Quaternion.identity, layerRendererObj.transform);

                    tileObj.TryGetComponent<Tile>(out Tile tile);
                    layerTiles.Add(tile);

                    tileObj.name = $"Tile_{x}_{y}";
                }
            }

            tiles.Add(i, layerTiles);
        }

        AdjustTileCount(levelData.tilePrefab);
        MarkBlockedTiles();
        GenerateScrews(levelData);
    }

    private void AdjustTileCount(GameObject tilePrefab)
    {
        int totalTileCount = GetTotalTileCount();
        int remainder = totalTileCount % 3;

        if (remainder == 0) return;

        int tilesToAdd = 3 - remainder;
        AddAdjustmentLayer(tilesToAdd, tilePrefab);

        Debug.Log($"Added Adjustment Layer with {tilesToAdd} tile(s) to ensure divisibility by 3.");
    }

    private void AddAdjustmentLayer(int tileCount, GameObject tilePrefab)
    {
        int adjustmentLayerNum = tiles.Count > 0 ? tiles.Count : 0;
        GameObject adjustmentLayerObj = CreateTileRendererLayer(adjustmentLayerNum);

        float startPosX = -(tileCount - 1) * (1 + tileSpacing) / 2f;

        List<Tile> tilesToAdd = new List<Tile>();

        for (int i = 0; i < tileCount; i++)
        {
            Vector3 tilePosition = new Vector3(
                startPosX + i * (1 + tileSpacing),
                0,
                adjustmentLayerNum * layerSpacing
            );

            GameObject tileObj = Instantiate(tilePrefab, tilePosition, Quaternion.identity, adjustmentLayerObj.transform);

            tileObj.TryGetComponent<Tile>(out Tile tile);
            tilesToAdd.Add(tile);

            tileObj.name = $"AdjustmentTile_{i}";
        }

        tiles.Add(adjustmentLayerNum, tilesToAdd);
    }

    private void MarkBlockedTiles()
    {
        foreach (var layerTiles in tiles)
        {
            var tiles = layerTiles.Value;

            foreach (var tile in tiles)
            {
                Vector3 tileBoxPos = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z + -0.5f);
                Collider[] colliders = Physics.OverlapBox(tileBoxPos, tile.gameObject.transform.localScale / 2, Quaternion.identity, tileLayer);

                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject == tile.gameObject) continue;

                    tile.SetTileState(false);
                    //Debug.Log(tile.gameObject.name + " has hit : " + collider.name);
                }
            }
        }
    }

    private void GenerateScrews(LevelData levelData)
    {
        List<Color> screwColors = GenerateScrewColorGroups(levelData);
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

    private List<Color> GenerateScrewColorGroups(LevelData levelData)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (!Application.isPlaying) return;

        foreach (var layerTiles in tiles)
        {
            var tiles = layerTiles.Value;

            foreach (var tile in tiles)
            {
                Vector3 tileBoxPos = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z + -0.5f);
                Gizmos.DrawWireCube(tileBoxPos, tile.gameObject.transform.localScale);
            }
        }
    }

    private int GetTotalTileCount()
    {
        int totalTiles = 0;

        foreach (var layerTiles in tiles)
        {
            totalTiles += layerTiles.Value.Count;
        }

        return totalTiles;
    }
}