using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HexagonHelpers : MonoBehaviour
{

    public static List<Vector2Int> GetAllNeighbors(Vector2Int hex, int minX = 0, int maxX = int.MaxValue, int minY = 0, int maxY = int.MaxValue)
    {
        int xOffset = hex.y % 2 == 0 ? -1 : 0;

        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (hex.x > minX)
            neighbors.Add(new Vector2Int(hex.x - 1, hex.y));

        if (hex.x < maxX)
            neighbors.Add(new Vector2Int(hex.x + 1, hex.y));

        if (hex.y > minY)
        {
            neighbors.Add(new Vector2Int(hex.x + xOffset, hex.y - 1));

            if (hex.x < maxX)
                neighbors.Add(new Vector2Int(hex.x + 1 + xOffset, hex.y - 1));
        }

        if (hex.y < maxY)
        {
            neighbors.Add(new Vector2Int(hex.x + xOffset, hex.y + 1));

            if (hex.x < maxX)
                neighbors.Add(new Vector2Int(hex.x + 1 + xOffset, hex.y + 1));
        }

        return neighbors;
    }
}
