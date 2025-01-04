using System.Collections.Generic;
using UnityEngine;

public class TileGridGenerator : MonoBehaviour
{
    public GameObject tilePrefab;
    public int rows = 5;
    public int columns = 5;
    public int layers = 1;
    public float tileSpacing = 0.1f;

    private List<KeyValuePair<int, ITile>> tiles = new List<KeyValuePair<int, ITile>>();

    void Start()
    {
        for (int i = 0; i < layers; i++)
        {
            var layerRows = rows - i;
            var layerColumns = columns - i;

            GenerateGrid(layerRows, layerColumns, i);
        }
    }

    void GenerateGrid(int rows, int columns, int layer)
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
                    0
                );

                GameObject tileObj = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);

                tileObj.TryGetComponent<ITile>(out ITile tile);
                tiles.Add(new KeyValuePair<int, ITile>(layer, tile));

                tile.SetLayer(layer);
                if (layer + 1 != layers)
                {
                    tile.SetTileState(false);
                }


                tileObj.name = $"Tile_{x}_{y}";
            }
        }
    }
}