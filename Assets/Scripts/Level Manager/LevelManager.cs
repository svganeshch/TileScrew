using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public CustomLevelData[] customLevels;
    public ColorData[] colorData;
    public GameObject[] tilesPrefab;
    public int currentLevel = 1;
    public LevelData currentLevelData;

    public void GenerateLevel(TileGridGenerator tileGridGenerator)
    {
        currentLevelData = GenerateRandomBaseLevel();
        tileGridGenerator.GenerateTileGrid(currentLevelData);

        if (currentLevel > 10)
        {
            var customLevelData = customLevels[Random.Range(0, customLevels.Length - 1)];
            if (customLevelData.customGridCells.Count > 0)
            {
                tileGridGenerator.GenerateCustomTileGrid(customLevelData, currentLevelData);
            }
        }

        ValidateLevel(tileGridGenerator);

        tileGridGenerator.GenerateScrews();

        StartCoroutine(tileGridGenerator.TileGridTweenAnimate());
    }

    private void ValidateLevel(TileGridGenerator tileGridGenerator)
    {
        int totalTilesCount = tileGridGenerator.GetTotalTileCount();
        int remainder = totalTilesCount % 3;

        if (remainder == 0) return;

        int tilesToAdd = 3 - remainder;

        LevelData adjustMentLayerData = ScriptableObject.CreateInstance<LevelData>();
        adjustMentLayerData.layers = 0;
        adjustMentLayerData.rows = tilesToAdd;
        adjustMentLayerData.columns = 1;
        adjustMentLayerData.reducedRows = false;
        adjustMentLayerData.reducedColumns = false;
        adjustMentLayerData.tilePrefab = currentLevelData.tilePrefab;

        tileGridGenerator.GenerateTileGrid(adjustMentLayerData, false, true);
        Debug.Log($"Added Adjustment Layer with {tilesToAdd} tile(s) to ensure divisibility by 3.");
    }

    public void GenerateNextLevel(TileGridGenerator tileGridGenerator)
    {
        currentLevel++;
        GenerateLevel(tileGridGenerator);
    }

    private LevelData GenerateRandomBaseLevel()
    {
        LevelData randomLevel = ScriptableObject.CreateInstance<LevelData>();

        randomLevel.rows = Random.Range(4, 8);
        randomLevel.columns = Random.Range(4, 8);
        randomLevel.layers = Random.Range(1, 4);

        randomLevel.reducedRows = Random.value < 0.5f;
        randomLevel.reducedColumns = Random.value < 0.5f;

        randomLevel.tilePrefab = tilesPrefab[Random.Range(0, tilesPrefab.Length - 1)];

        return randomLevel;
    }
}
