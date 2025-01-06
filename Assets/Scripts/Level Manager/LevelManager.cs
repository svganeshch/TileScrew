using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelData[] levels;
    public ColorData[] colorData;
    public int currentLevelIndex = 0;
    public LevelData currentLevelData;

    public void GenerateLevel(TileGridGenerator tileGridGenerator)
    {
        tileGridGenerator.GenerateGrid(GetCurrentLevelData());
    }

    public LevelData GetCurrentLevelData()
    {
        return levels[currentLevelIndex];
    }
}
