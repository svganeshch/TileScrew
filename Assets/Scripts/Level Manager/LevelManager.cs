using DG.Tweening;
using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int MAX_ROWS = 8;
    public int MAX_COLUMNS = 8;

    public CustomLevelData[] customLevels;
    public ColorData[] colorData;
    public GameObject[] tilesPrefab;
    public int currentLevel = 1;
    public LevelData currentLevelData;

    public void GenerateLevel(TileGridGenerator tileGridGenerator)
    {
        GameManager.currentGameState = GameState.Active;

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

        randomLevel.rows = Random.Range(4, MAX_ROWS + 1);
        randomLevel.columns = Random.Range(4, MAX_COLUMNS + 1);
        randomLevel.layers = Random.Range(1, 4);

        randomLevel.reducedRows = Utils.GetRandomBool();
        randomLevel.reducedColumns = Utils.GetRandomBool();

        randomLevel.tilePrefab = tilesPrefab[Random.Range(0, tilesPrefab.Length - 1)];

        return randomLevel;
    }

    public IEnumerator ClearLevel()
    {
        DOTween.KillAll();

        bool gridCleared = GameManager.Instance.tileGridGenerator.ClearGrid();
        bool slotsCleared = GameManager.Instance.slotManager.ClearAllSlots();

        yield return new WaitUntil(() => gridCleared && slotsCleared);
    }
}
