using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public int rows;
    public int columns;
    public int layers = 1;

    public bool reducedRows = false;
    public bool reducedColumns = false;

    public GameObject tilePrefab;

    [HideInInspector] public List<Vector2Int> customGridCells = new List<Vector2Int>();
}