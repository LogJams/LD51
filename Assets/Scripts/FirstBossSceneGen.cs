using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class FirstBossSceneGen : MonoBehaviour
{

    public GameObject dirtTile;
    public GameObject waterTile;
    public GameObject grassTile;

    public GameObject pathOutline;

    public GameObject player;

    private static int numberTilesX = 20;
    private static int numberTilesZ = 20;

    private List<GameObject> _oldPathIndicators = new List<GameObject>();

    private enum FloorTypeNames
    {
        dirt = 0,
        grass,
        water
    };

    private Dictionary<FloorTypeNames, GameObject> TileMapper;

    private int[,] FloorTypes = new int[numberTilesX,numberTilesZ];

    // Start is called before the first frame update
    void Start()
    {
        GenerateTileDictionary();
        GenerateFloorTiles();
        GeneratePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        ClearPath();
        ShowPath();
    }

    void ClearPath()
    {
        foreach (GameObject obj in _oldPathIndicators)
            Destroy(obj);
    }

    void GenerateTileDictionary()
    {
        TileMapper = new Dictionary<FloorTypeNames, GameObject>() {
            { FloorTypeNames.dirt, dirtTile},
            { FloorTypeNames.grass, grassTile},
            { FloorTypeNames.water, waterTile}
        };
    }

    void GenerateFloorTiles()
    {
        for (int j = 0; j < numberTilesZ; j++)
        {
            for (int i = 0; i < numberTilesX; i++)
            {
                // Draw a random tile
                FloorTypes[i, j] = GetRandomFloorType();

                // Instantiate it
                GameObject newTile = Instantiate(TileMapper[(FloorTypeNames)FloorTypes[i,j]]) as GameObject;
                SetPosition(newTile, i, 0, j);
            }
        }
    }

    private void SetPosition(GameObject obj, int indxX, int y, int indxZ)
    {
        float iVal = indxX;
        if (indxZ % 2 == 1)
            iVal += 0.5f;
        obj.transform.position = new Vector3(iVal, y, indxZ * Mathf.Sqrt(3) / 2);
    }

    private int GetRandomFloorType()
    {
        if (UnityEngine.Random.Range(0, 10) < 3)
            return 0;


        if (UnityEngine.Random.Range(0, 10) < 8)
            return 1;


        return 2;
    }

    void GeneratePlayer()
    {
        GameObject mainPlayer = Instantiate(player) as GameObject;
        mainPlayer.transform.position = new Vector3(10, 2, 10);
    }

    void ShowPath()
    {
        // Show the potential path
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            var builder = new StringBuilder();

            Vector2Int start = GetTileIndexFromObject(player.transform);
            Vector2Int goal = GetTileIndexFromObject(hit.transform);

            // builder.AppendFormat("Name: {0}, Position = ({1},  {2}) ", hit.collider.gameObject.name, goal.x, goal.y);
            // Debug.Log(builder.ToString());

            List<Vector2Int> path = FindPath(start, goal);

            Debug.Log(path.Count);
            foreach (Vector2Int wayPoint in path)
            {
                GameObject newTile = Instantiate(pathOutline) as GameObject;
                SetPosition(newTile, wayPoint.x, 2, wayPoint.y);
            }
        }
    }

    /// <summary>
    /// A* following https://www.redblobgames.com/pathfinding/a-star/introduction.html
    /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <returns></returns>
    List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal) 
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(start);
        Dictionary<Vector2Int, Vector2Int> came_from = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, float> cost_so_far = new Dictionary<Vector2Int, float>();

        List<Vector2Int> path = new List<Vector2Int>();

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

                    if (current == start)
                        return path;
                }
            }

            foreach (Vector2Int neighbor in GetNeighbors(current))
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

    float heuristic(Vector2Int start, Vector2Int goal)
    {
        return Mathf.Abs(start.x - goal.x) + Mathf.Abs(start.y - goal.y);
    }

    int GetFloorType(int x, int y)
    {
        try
        {
            return FloorTypes[x, y];
        } catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return 0;
        }
    }

    List<Vector2Int> GetNeighbors(Vector2Int hex)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        if (hex.x > 0)
        {
            if (IsWalkable(GetFloorType(hex.x-1, hex.y)))
                neighbors.Add(new Vector2Int(hex.x - 1, hex.y));
        }

        if (hex.x < numberTilesX - 1)
        {
            if (IsWalkable(GetFloorType(hex.x + 1, hex.y)))
                neighbors.Add(new Vector2Int(hex.x + 1, hex.y));
        }

        if (hex.y > 0)
        {
            if (IsWalkable(GetFloorType(hex.x, hex.y - 1)))
                neighbors.Add(new Vector2Int(hex.x, hex.y - 1));

            if (hex.x < numberTilesX - 1)
            {
                if (IsWalkable(GetFloorType(hex.x + 1, hex.y - 1)))
                    neighbors.Add(new Vector2Int(hex.x + 1, hex.y - 1));
            }
        }

        if (hex.y < numberTilesZ - 1)
        {
            if (IsWalkable(GetFloorType(hex.x, hex.y + 1)))
                neighbors.Add(new Vector2Int(hex.x, hex.y + 1));

            if (hex.x < numberTilesX - 1)
            {
                if (IsWalkable(GetFloorType(hex.x + 1, hex.y + 1)))
                    neighbors.Add(new Vector2Int(hex.x + 1, hex.y + 1));
            }
        }

        return neighbors;
    }

    bool IsWalkable(int floorType)
    {
        return floorType < 2;
    }

    Vector2Int GetTileIndexFromObject(Transform tile)
    {
        // Every x position is either an integer or an integer + 0.5
        // Either way, the integer represents its coordinate (so we can subtract a small amout and round)
        int xIdx = Mathf.RoundToInt(tile.transform.position.x - 0.1f);
        int zIdx = Mathf.RoundToInt(tile.transform.position.z * 2 / Mathf.Sqrt(3));

        return new Vector2Int(xIdx, zIdx);
    }
}
