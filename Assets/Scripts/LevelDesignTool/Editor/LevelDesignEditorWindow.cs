using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelDesignEditorWindow : EditorWindow
{
    private const int gridSize = 9;
    private bool[,] gridState = new bool[gridSize, gridSize];
    private Vector2 scrollPos;
    private string fileName = "NewGridData";
    private CustomLevelData loadedData;
    private const float cellSize = 25f;

    [MenuItem("Tools/Level Design Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelDesignEditorWindow>("Level Design Editor");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Level Design Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        fileName = EditorGUILayout.TextField("File Name", fileName);

        loadedData = (CustomLevelData)EditorGUILayout.ObjectField("Load Existing Data", loadedData, typeof(CustomLevelData), false);
        if (GUILayout.Button("Load Grid Data"))
        {
            LoadGridData();
        }

        EditorGUILayout.Space();

        float gridWidth = gridSize * cellSize;
        float offsetX = Mathf.Max(0, (position.width - gridWidth) / 2 - 30);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUILayout.BeginHorizontal();
        GUILayout.Space(offsetX);

        // Draw column numbers
        GUILayout.BeginVertical();
        GUILayout.Space(cellSize);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(cellSize);
        for (int x = 0; x < gridSize; x++)
        {
            GUILayout.Label(x.ToString(), GUILayout.Width(cellSize), GUILayout.Height(cellSize / 2));
        }
        EditorGUILayout.EndHorizontal();

        // Draw grid with row numbers
        for (int y = 0; y < gridSize; y++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(y.ToString(), GUILayout.Width(cellSize), GUILayout.Height(cellSize)); // Row numbers
            for (int x = 0; x < gridSize; x++)
            {
                bool currentState = gridState[x, y];
                gridState[x, y] = GUILayout.Toggle(currentState, GUIContent.none, GUILayout.Width(cellSize), GUILayout.Height(cellSize));
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        // Buttons
        if (GUILayout.Button("Clear Grid"))
        {
            ClearGrid();
        }

        if (GUILayout.Button("Save Grid Data"))
        {
            SaveGridData();
        }
    }

    private void ClearGrid()
    {
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                gridState[x, y] = false;
            }
        }
    }

    private void SaveGridData()
    {
        List<Vector2Int> selectedCells = new List<Vector2Int>();
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                if (gridState[x, y])
                {
                    selectedCells.Add(new Vector2Int(x, y));
                }
            }
        }

        string path = $"Assets/Data/Custom Level Data/{fileName}.asset";

        // Check if overwriting or creating a new file
        if (System.IO.File.Exists(path) && (loadedData == null || AssetDatabase.GetAssetPath(loadedData) != path))
        {
            bool overwrite = EditorUtility.DisplayDialog(
                "File Exists",
                $"A file named '{fileName}.asset' already exists. Do you want to overwrite it?",
                "Overwrite",
                "Cancel"
            );

            if (!overwrite)
            {
                Debug.Log("Save operation canceled.");
                return;
            }
        }

        CustomLevelData levelDesignData;

        if (loadedData != null && AssetDatabase.GetAssetPath(loadedData) == path)
        {
            levelDesignData = loadedData;
        }
        else
        {
            levelDesignData = CreateInstance<CustomLevelData>();
            AssetDatabase.CreateAsset(levelDesignData, path);
        }

        levelDesignData.customGridCells = selectedCells;
        EditorUtility.SetDirty(levelDesignData);
        AssetDatabase.SaveAssets();

        Debug.Log($"Grid data saved to {path}");
    }

    private void LoadGridData()
    {
        if (loadedData == null)
        {
            Debug.LogWarning("No grid data selected to load.");
            return;
        }

        ClearGrid();

        foreach (var cell in loadedData.customGridCells)
        {
            if (cell.x >= 0 && cell.x < gridSize && cell.y >= 0 && cell.y < gridSize)
            {
                gridState[cell.x, cell.y] = true;
            }
        }

        string loadedPath = AssetDatabase.GetAssetPath(loadedData);
        fileName = System.IO.Path.GetFileNameWithoutExtension(loadedPath);

        Debug.Log("Grid data loaded successfully.");
    }
}