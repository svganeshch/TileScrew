using DG.Tweening;
using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int MAX_ROWS = 8;
    public int MAX_COLUMNS = 8;
    public int MAX_LAYERS = 6;

    public CustomLevelData[] customLevels;
    public ColorData[] colorData;
    public GameObject[] tilesPrefab;
    public int currentLevel = 1;
    public LevelData currentLevelData;

    private void Awake()
    {
        UIManager.Instance.onLevelChangeEvent.Invoke(currentLevel.ToString());
    }

    public void GenerateLevel(TileGridGenerator tileGridGenerator)
    {
        GameManager.currentGameState = GameState.Active;

        currentLevelData = GenerateRandomBaseLevel();
        tileGridGenerator.GenerateTileGrid(currentLevelData);

        if (currentLevel > 10)
        {
            var customLevelData = customLevels[Random.Range(0, customLevels.Length - 1)];

            if (customLevelData != null)
            {
                if (customLevelData.customGridCells.Count > 0)
                {
                    tileGridGenerator.GenerateCustomTileGrid(customLevelData, currentLevelData);
                }
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

        UIManager.Instance.onLevelChangeEvent.Invoke(currentLevel.ToString());
    }

    private LevelData GenerateRandomBaseLevel()
    {
        LevelData randomLevel = ScriptableObject.CreateInstance<LevelData>();

        // Gradually increase the range of rows and columns with the level
        // Increment min rows and columns every level
        int minRows = Mathf.Clamp(currentLevel, 2, MAX_ROWS);
        int maxRows = Mathf.Clamp(currentLevel, minRows, MAX_ROWS);
        int minColumns = Mathf.Clamp(currentLevel, 2, MAX_COLUMNS);
        int maxColumns = Mathf.Clamp(currentLevel, minColumns, MAX_COLUMNS);

        // Generate rows and columns within the dynamic range
        randomLevel.rows = Random.Range(minRows, maxRows + 1);
        randomLevel.columns = Random.Range(minColumns, maxColumns + 1);

        // Gradually increase layers based on level
        // Add layers every 10 levels up to a max
        int maxLayers = Mathf.Min(1 + currentLevel / 10, MAX_LAYERS);
        randomLevel.layers = Random.Range(1, maxLayers + 1);

        // Toggle Ice Tiles more often starting from level 10
        if (currentLevel > 20)
        {
            float iceTilesChance = Mathf.Clamp01(0.6f + (currentLevel * 0.02f));
            randomLevel.hasIceTiles = Utils.GetRandomBool(iceTilesChance);
        }

        randomLevel.tilePrefab = tilesPrefab[Random.Range(0, tilesPrefab.Length)];

        return randomLevel;
    }

    public bool CalculateRandomLayerChance()
    {
        // Toggle reduced / increased rows and columns more often in higher levels
        float reducedChance = Mathf.Clamp01(0.5f + (currentLevel * 0.02f));
        return Utils.GetRandomBool(reducedChance);
    }

    public IEnumerator ClearLevel()
    {
        DOTween.KillAll();

        bool gridCleared = GameManager.Instance.tileGridGenerator.ClearGrid();
        bool slotsCleared = GameManager.Instance.slotManager.ClearAllSlots();

        yield return new WaitUntil(() => gridCleared && slotsCleared);
    }
}
