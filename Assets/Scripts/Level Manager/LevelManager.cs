using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelData[] levels;
    public ColorData[] colorData;
    public int currentLevelIndex;
    public LevelData currentLevelData;

    private void Awake()
    {
        currentLevelIndex = 0;
        currentLevelData = levels[currentLevelIndex];
    }

    public void GenerateLevel(TileGridGenerator tileGridGenerator)
    {
        for (int i = 0; i <= currentLevelData.layers; i++)
        {
            tileGridGenerator.GenerateGrid(GetCurrentLevelData());
        }

        ValidateLevel(tileGridGenerator);
        tileGridGenerator.GenerateScrews();
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

        tileGridGenerator.GenerateGrid(adjustMentLayerData);

        Debug.Log($"Added Adjustment Layer with {tilesToAdd} tile(s) to ensure divisibility by 3.");
    }

    public LevelData GetCurrentLevelData()
    {
        return levels[currentLevelIndex];
    }
}
