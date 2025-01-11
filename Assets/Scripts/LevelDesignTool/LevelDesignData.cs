using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDesignData", menuName = "Custom Tools/Level Design Data", order = 1)]
public class LevelDesignData : ScriptableObject
{
    public List<Vector2Int> selectedCells = new List<Vector2Int>();
}