using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManagerTool))]
public class LevelManagerToolEditor : Editor
{
    public string levelsPath = $"Assets/Data/Levels/Resources/";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelManagerTool tool = (LevelManagerTool)target;

        if (GUILayout.Button("Generate"))
        {
            GenerateLevelData(tool);
        }

        if (GUILayout.Button("Load Level"))
        {
            tool.LoadLevelData();
        }

        if (GUILayout.Button("Clear"))
        {
            tool.ClearLevel();
        }

        if (GUILayout.Button("Save"))
        {
            SaveLevelAsset(tool);
        }
    }

    private void GenerateLevelData(LevelManagerTool levelManagerTool)
    {
        if (levelManagerTool.loadLevelData != null)
        {
            var currentLevelData = GenerateRandomBaseLevel(levelManagerTool);
            SaveLevelAsset(levelManagerTool, currentLevelData);

            levelManagerTool.loadLevelData = currentLevelData;

            return;
        }

        for (int i = 0; i < levelManagerTool.numberOfLevelsToProduce; i++)
        {
            var currentLevelData = GenerateRandomBaseLevel(levelManagerTool);
            SaveLevelAsset(levelManagerTool, currentLevelData);

            levelManagerTool.currentLevelNum++;
        }
    }

    private void SaveLevelAsset(LevelManagerTool levelManagerTool, LevelData levelData = null)
    {
        if (levelData == null)
        {
            levelData = levelManagerTool.loadLevelData;
        }

        AssetDatabase.CreateAsset(levelData, $"{levelsPath}{levelManagerTool.currentLevelNum}.asset");
    }

    public LevelData GenerateRandomBaseLevel(LevelManagerTool levelManagerTool)
    {
        LevelData randomLevel = ScriptableObject.CreateInstance<LevelData>();

        // Set a random seed if not already set
        randomLevel.seed = Random.Range(int.MinValue, int.MaxValue);

        // Generate rows and columns within the dynamic range
        randomLevel.rows = Random.Range(4, LevelManagerTool.MAX_ROWS + 1);
        randomLevel.columns = Random.Range(4, LevelManagerTool.MAX_COLUMNS + 1);

        // Gradually increase layers based on level
        // Add layers every 10 levels up to a max
        int maxLayers = Mathf.Min(1 + levelManagerTool.currentLevelNum / 10, LevelManagerTool.MAX_LAYERS);
        randomLevel.layers = Random.Range(1, maxLayers + 1);

        // Toggle Ice Tiles more often starting from level 10
        if (levelManagerTool.currentLevelNum > 20)
        {
            float iceTilesChance = Mathf.Clamp01(0.6f + (levelManagerTool.currentLevelNum * 0.02f));
            randomLevel.hasIceTiles = Utils.GetRandomBool(iceTilesChance);
        }

        randomLevel.tilePrefab = levelManagerTool.tilesPrefab[Random.Range(0, levelManagerTool.tilesPrefab.Length)];

        return randomLevel;
    }
}