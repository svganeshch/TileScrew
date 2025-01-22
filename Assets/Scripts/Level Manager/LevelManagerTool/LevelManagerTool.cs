using UnityEngine;

public class LevelManagerTool : MonoBehaviour
{
    public LevelManager levelManager;
    public TileGridGenerator tileGridGenerator;

    public static int MAX_ROWS = 8;
    public static int MAX_COLUMNS = 8;
    public static int MAX_LAYERS = 6;

    public int numberOfLevelsToProduce = 1;

    public LevelData loadLevelData;

    public int currentLevelNum = 1;

    public GameObject[] tilesPrefab;

    public void LoadLevelData()
    {
        tileGridGenerator.GenerateTileGrid(loadLevelData);

        EnableTiles();
        tileGridGenerator.GenerateScrews();
    }

    public void ClearLevel()
    {
        foreach (var tilesValuePair in tileGridGenerator.tiles)
        {
            var layerTiles = tilesValuePair.Value;

            foreach (var tile in layerTiles)
            {
                DestroyImmediate(tile.gameObject);
            }

            DestroyImmediate(tilesValuePair.Key);
        }

        tileGridGenerator.tiles.Clear();
    }

    private void EnableTiles()
    {
        foreach (var tilesValuePair in tileGridGenerator.tiles)
        {
            var layerTiles = tilesValuePair.Value;

            foreach (var tile in layerTiles)
            {
                tile.gameObject.SetActive(true);
            }
        }
    }
}
