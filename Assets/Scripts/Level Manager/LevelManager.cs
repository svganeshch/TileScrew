using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelData[] levels;
    public ColorData[] colorData;
    public int currentLevelIndex;
    public LevelData currentLevelData;

    private void Awake()
    {
        currentLevelData = levels[currentLevelIndex];
    }

    public void GenerateLevel(TileGridGenerator tileGridGenerator)
    {
        currentLevelData = GetCurrentLevelData();
        tileGridGenerator.GenerateTileGrid(currentLevelData);

        if (currentLevelData.customGridCells.Count > 0)
        {
            tileGridGenerator.GenerateTileGrid(currentLevelData, true);
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

        tileGridGenerator.GenerateTileGrid(adjustMentLayerData);
        Debug.Log($"Added Adjustment Layer with {tilesToAdd} tile(s) to ensure divisibility by 3.");
    }

    public void GenerateNextLevel(TileGridGenerator tileGridGenerator)
    {
        currentLevelIndex++;
        GenerateLevel(tileGridGenerator);
    }

    public LevelData GetCurrentLevelData()
    {
        return levels[currentLevelIndex];
    }
}
