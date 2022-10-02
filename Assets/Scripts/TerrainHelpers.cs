using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainHelpers : MonoBehaviour
{
    public enum TerrainTypes
    {
        undefined = -1,
        stone = 0,
        path = 1,
        grass = 2,
        water = 3,
        cliff = 4
    };

    public static bool IsWalkable(int floorType)
    {
        return floorType <= 2 && floorType >= 0;
    }

    public static int GetFloorType(int[,] map, int x, int y)
    {
        try
        {
            return map[x, y];
        }
        catch (Exception ex)
        {
            return (int)TerrainTypes.undefined;
        }
    }

    public static int GetFloorType(int[,] map, Vector2Int idxs)
    {
        return GetFloorType(map, idxs.x, idxs.y);
    }
}
