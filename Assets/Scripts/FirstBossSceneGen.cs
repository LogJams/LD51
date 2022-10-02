using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static TerrainHelpers;
using static HexagonHelpers;

public class FirstBossSceneGen : MonoBehaviour
{

    public GameObject dirtTile;
    public GameObject waterTile;
    public GameObject grassTile;

    public GameObject pathOutline;

    public GameObject playerPrefab;

    private GameObject player;
    private CharacterController controller;

    private List<Vector2Int> currentpath;

    private static int numberTilesX = 20;
    private static int numberTilesZ = 20;

    private bool isMoving = false;
    private float movingSpeed = 3.0f;

    private List<GameObject> _oldPathIndicators = new List<GameObject>();

    private Dictionary<TerrainTypes, GameObject> TileMapper;

    private int[,] TerrainTypeMap = new int[numberTilesX,numberTilesZ];

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
        if (!isMoving)
        {
            ClearPath();
            ShowPath();
        } else
        {
            MovePlayer();
        }
    }

    void ClearPath()
    {
        foreach (GameObject obj in _oldPathIndicators)
            Destroy(obj);

        _oldPathIndicators.Clear();
    }

    void GenerateTileDictionary()
    {
        TileMapper = new Dictionary<TerrainTypes, GameObject>() {
            { TerrainTypes.path, dirtTile},
            { TerrainTypes.grass, grassTile},
            { TerrainTypes.water, waterTile}
        };
    }

    void GenerateFloorTiles()
    {
        for (int j = 0; j < numberTilesZ; j++)
        {
            for (int i = 0; i < numberTilesX; i++)
            {
                // Draw a random tile
                TerrainTypeMap[i, j] = GetRandomFloorType();

                // Instantiate it
                GameObject newTile = Instantiate(TileMapper[(TerrainTypes)TerrainTypeMap[i,j]]) as GameObject;
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
            return (int)TerrainTypes.grass;

        if (UnityEngine.Random.Range(0, 10) < 8)
            return (int)TerrainTypes.path;

        return (int)TerrainTypes.water;
    }

    void GeneratePlayer()
    {
        player = Instantiate(playerPrefab) as GameObject;
        player.transform.position = new Vector3(numberTilesX/2.0f, 2, numberTilesZ*Mathf.Sqrt(3)/4);
        controller = player.GetComponent<CharacterController>();
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

            List<Vector2Int> path = FindPath(TerrainTypeMap, start, goal, 0, numberTilesX - 1, 0, numberTilesZ - 1);

            Debug.Log(path.Count);
            foreach (Vector2Int wayPoint in path)
            {
                GameObject newTile = Instantiate(pathOutline) as GameObject;
                SetPosition(newTile, wayPoint.x, 0, wayPoint.y);

                _oldPathIndicators.Add(newTile);
            }

            currentpath = path;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (currentpath.Count > 0)
                isMoving = true;
        }
    }

    void VisualizeNeighbors()
    {
        // Show the potential path
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            Vector2Int tile = GetTileIndexFromObject(hit.transform);

            foreach (Vector2Int neighbor in GetWalkableNeighbors(TerrainTypeMap, tile, 0, numberTilesX - 1, 0, numberTilesZ - 1))
            {
                GameObject newTile = Instantiate(pathOutline) as GameObject;
                SetPosition(newTile, neighbor.x, 0, neighbor.y);

                _oldPathIndicators.Add(newTile);
            }
        }
    }

    void MovePlayer()
    {
        if (!isMoving)
            return;

        if (currentpath.Count > 0)
        {
            Vector2Int goalPosition = currentpath.Last();
            Vector3 tilePosition = GetPositionFromIndex(goalPosition);
            Vector3 projectedPlayerPosition = new Vector3(player.transform.position.x, 0, player.transform.position.z);

            if ((tilePosition - projectedPlayerPosition).magnitude < 0.1)
            {
                currentpath.RemoveAt(currentpath.Count - 1);

                if (currentpath.Count == 0)
                    isMoving = false;
            } else
            {
                controller.Move((tilePosition - projectedPlayerPosition).normalized * Time.deltaTime * movingSpeed);
            }
        } else {
            // Shouldn't enter this, but let's make sure 
            throw new Exception("Why are we entering this code?");
        }
    }
}
