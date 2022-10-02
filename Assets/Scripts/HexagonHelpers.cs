using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TerrainHelpers;

public class HexagonHelpers : MonoBehaviour
{
    /// <summary>
    /// Given a tile index, returns all neighboring tile indices
    /// </summary>
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

    /// <summary>
    /// Given a tile index, returns all walkable neighboring tile indices
    /// </summary>
    public static List<Vector2Int> GetWalkableNeighbors(int[,] map, Vector2Int hex, int minX = 0, int maxX = int.MaxValue, int minY = 0, int maxY = int.MaxValue)
    {
        List<Vector2Int> allNeighbors = GetAllNeighbors(hex, minX, maxX, minY, maxY);
        List<Vector2Int> neighbors = new List<Vector2Int>();

        foreach (Vector2Int neighbor in allNeighbors)
        {
            if (IsWalkable(GetFloorType(map, neighbor)))
                neighbors.Add(neighbor);

        }

        return neighbors;
    }

    public static Vector2Int GetTileIndexFromObject(Transform tile)
    {
        // Every x position is either an integer or an integer + 0.5
        // Either way, the integer represents its coordinate (so we can subtract a small amout and round)
        int xIdx = Mathf.RoundToInt(tile.transform.position.x - 0.1f);
        int zIdx = Mathf.RoundToInt(tile.transform.position.z * 2 / Mathf.Sqrt(3));

        return new Vector2Int(xIdx, zIdx);
    }

    public static Vector3 GetPositionFromIndex(Vector2Int tileIdx)
    {
        float xOffset = tileIdx.y % 2 == 0 ? 0 : 0.5f;
        return new Vector3(tileIdx.x + xOffset, 0, tileIdx.y * Mathf.Sqrt(3) / 2);
    }

    /// <summary>
    /// A* following https://www.redblobgames.com/pathfinding/a-star/introduction.html
    public static List<Vector2Int> FindPath(int[,] map, Vector2Int start, Vector2Int goal, int minX = 0, int maxX = int.MaxValue, int minY = 0, int maxY = int.MaxValue)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        if (start == goal || !IsWalkable(GetFloorType(map, goal)))
            return path;

        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(start);
        Dictionary<Vector2Int, Vector2Int> came_from = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, float> cost_so_far = new Dictionary<Vector2Int, float>();

        cost_so_far.Add(start, 0);

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            if (current == goal)
            {
                while (true)
                {
                    path.Add(current);
                    current = came_from[current];

                    if (!IsWalkable(GetFloorType(map, current)))
                        path.Clear();

                    if (current == start)
                        return path;
                }
            }

            foreach (Vector2Int neighbor in GetWalkableNeighbors(map, current, minX, maxX, minY, maxY))
            {
                // Assuming one unit cost per neighbor
                float new_cost = cost_so_far[current] + 1;

                // Ignore duplicate field
                if (cost_so_far.ContainsKey(neighbor))
                    continue;

                cost_so_far[neighbor] = new_cost;

                frontier.Enqueue(neighbor);
                came_from[neighbor] = current;
            }
        }

        return path;
    }

    public static void SetPosition(GameObject obj, int indxX, int y, int indxZ)
    {
        float iVal = indxX;
        if (indxZ % 2 == 1)
            iVal += 0.5f;
        obj.transform.position = new Vector3(iVal, y, indxZ * Mathf.Sqrt(3) / 2);
    }

}
