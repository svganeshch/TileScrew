using DG.Tweening;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public CustomLevelData[] customLevels;
    public ColorData[] colorData;
    public int currentLevel = 1;
    public LevelData currentLevelData;

    private void Awake()
    {
        UIManager.Instance.onLevelChangeEvent.Invoke(currentLevel.ToString());
    }

    public void GenerateLevel(TileGridGenerator tileGridGenerator)
    {
        GameManager.currentGameState = GameState.Active;

        LevelData existingLevelData = Resources.Load<LevelData>(currentLevel.ToString());
        if (existingLevelData != null)
        {
            currentLevelData = existingLevelData;
        }
        else
        {
            Debug.LogError($"Level data not found for level {currentLevel}!!");
        }

        tileGridGenerator.GenerateTileGrid(currentLevelData);

        if (currentLevel > 10)
        {
            Random.InitState((int)System.DateTime.Now.Ticks);
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
