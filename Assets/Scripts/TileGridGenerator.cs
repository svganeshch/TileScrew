using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TileGridGenerator : MonoBehaviour
{
    public GameObject tileLayerPrefab;
    public GameObject tilePrefab;
    public int rows = 5;
    public int columns = 5;
    public int layers = 1;
    public float tileSpacing = 0.1f;
    public float layerSpacing = -0.25f;

    private List<KeyValuePair<int, ITile>> tiles = new List<KeyValuePair<int, ITile>>();

    void Start()
    {
        for (int i = 0; i < layers; i++)
        {
            var layerRows = rows - i;
            var layerColumns = columns - i;

            var layerObj = CreateTileRendererLayer(i);
            GenerateGrid(layerRows, layerColumns, i, layerObj);
        }
    }

    private GameObject CreateTileRendererLayer(int layerNum)
    {
        GameObject tileLayerObj = Instantiate(tileLayerPrefab, transform);
        tileLayerObj.name = "Layer " + layerNum;

        tileLayerObj.GetComponent<SortingGroup>().sortingOrder = layerNum;

        return tileLayerObj;
    }

    void GenerateGrid(int rows, int columns, int layerNum, GameObject layerParentObj)
    {
        float totalWidth = (columns - 1) * (1 + tileSpacing);
        float totalHeight = (rows - 1) * (1 + tileSpacing);

        float startPosX = -totalWidth / 2f;
        float startPosY = totalHeight / 2f;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3 tilePosition = new Vector3(
                    startPosX + x * (1 + tileSpacing),
                    startPosY - y * (1 + tileSpacing),
                    layerNum * layerSpacing
                );

                GameObject tileObj = Instantiate(tilePrefab, tilePosition, Quaternion.identity, layerParentObj.transform);

                tileObj.TryGetComponent<ITile>(out ITile tile);
                tiles.Add(new KeyValuePair<int, ITile>(layerNum, tile));

                //tile.SetLayer(layerNum);
                if (layerNum + 1 != layers)
                {
                    tile.SetTileState(false);
                }


                tileObj.name = $"Tile_{x}_{y}";
            }
        }
    }
}