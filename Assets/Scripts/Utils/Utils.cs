using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static void ShuffleList<T>(ref List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public static (int rows, int columns) GetGridDimensions(List<Vector2Int> selectedCells)
    {
        if (selectedCells == null || selectedCells.Count == 0)
        {
            return (0, 0);
        }

        int maxRow = int.MinValue;
        int maxColumn = int.MinValue;

        foreach (var cell in selectedCells)
        {
            if (cell.y > maxRow) maxRow = cell.y;
            if (cell.x > maxColumn) maxColumn = cell.x;
        }

        return (maxRow + 1, maxColumn + 1);
    }

    public static HashSet<Vector2Int> GetInnerLayerCells(HashSet<Vector2Int> currentCells)
    {
        HashSet<Vector2Int> innerCells = new HashSet<Vector2Int>();

        foreach (var cell in currentCells)
        {
            bool isInnerCell = true;

            Vector2Int[] neighbors = {
            new Vector2Int(cell.x + 1, cell.y),
            new Vector2Int(cell.x - 1, cell.y),
            new Vector2Int(cell.x, cell.y + 1),
            new Vector2Int(cell.x, cell.y - 1)
        };

            foreach (var neighbor in neighbors)
            {
                if (!currentCells.Contains(neighbor))
                {
                    isInnerCell = false;
                    break;
                }
            }

            if (isInnerCell)
            {
                innerCells.Add(cell);
            }
        }

        return innerCells;
    }

    public static bool GetRandomBool(float chance = 0.5f)
    {
        return Random.value < chance;
    }
}
