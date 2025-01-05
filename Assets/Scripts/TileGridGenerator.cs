using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TileGridGenerator : MonoBehaviour
{
    public GameObject tileLayerPrefab;
 
    public float tileSpacing = 0.1f;
    public float layerSpacing = -0.25f;

    private List<KeyValuePair<int, ITile>> tiles = new List<KeyValuePair<int, ITile>>();

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

                    tileObj.TryGetComponent<ITile>(out ITile tile);
                    tiles.Add(new KeyValuePair<int, ITile>(i, tile));

                    //tile.SetLayer(layerNum);
                    if (i > 0)
                    {
                        tile.SetTileState(false);
                    }


                    tileObj.name = $"Tile_{x}_{y}";
                }
            }
        }
    }

    private GameObject CreateTileRendererLayer(int layerNum)
    {
        GameObject tileLayerObj = Instantiate(tileLayerPrefab, transform);
        tileLayerObj.name = "Layer " + layerNum;

        tileLayerObj.GetComponent<SortingGroup>().sortingOrder = layerNum;

        return tileLayerObj;
    }
}