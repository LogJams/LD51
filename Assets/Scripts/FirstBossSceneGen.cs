using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static TerrainHelpers;
using static HexagonHelpers;

using UnityEngine.EventSystems;

public class FirstBossSceneGen : MonoBehaviour
{
    public GameObject dirtTile;
    public GameObject waterTile;
    public GameObject grassTile;
    public GameObject cliffTile;

    public GameObject pathOutline;

    public GameObject playerPrefab;

    public GameObject turretBoss;

    public UI_TimerPanel timer;

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
        timer.OnTimerEnd += EndTurn;
    }

    // Update is called once per frame
    void Update()
    {

        if (!isMoving)
        {
            //do nothing if we are mousing over the UI or it's not the player's turn
            if (!EventSystem.current.IsPointerOverGameObject() && timer.PlayerCanAct()) {
                ClearPath();
                ShowPath();
            }
        } else
        {
            MovePlayer();
        }
    }


    public void EndTurn(System.Object src, EventArgs e) {
        Vector2Int goalPos = currentpath[currentpath.Count - 1];
        currentpath.Clear();

        currentpath.Add(goalPos);

        for (int i = _oldPathIndicators.Count-1; i >= 0; i--) {
            Destroy(_oldPathIndicators[i]);
        }
        _oldPathIndicators.Clear();

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
            { TerrainTypes.water, waterTile},
            { TerrainTypes.cliff, cliffTile}
        };
    }

    void GenerateFloorTiles() {

        //boundary of CLIFFS
        for (int i = 0; i < numberTilesX; i++) {
            TerrainTypeMap[i, 0] = (int)TerrainTypes.cliff;
            TerrainTypeMap[i, numberTilesZ-1] = (int)TerrainTypes.cliff;
        }
        for (int j = 0; j < numberTilesZ; j++) {
            TerrainTypeMap[0, j] = (int)TerrainTypes.cliff;
            TerrainTypeMap[numberTilesX - 1, j] = (int)TerrainTypes.cliff;
        }

        int bossX = numberTilesX / 2;
        int bossZ = numberTilesZ - 4;


        //fill in the inside
        for (int j = 1; j < numberTilesZ-1; j++) {
            for (int i = 1; i < numberTilesX-1; i++) {
                // Draw a random tile
                TerrainTypeMap[i, j] = GetRandomFloorType();
            }
        }



        //generate the final tilemap
        for (int j = 0; j < numberTilesZ; j++) {
            for (int i = 0; i < numberTilesX; i++) {
                // Instantiate it
                GameObject newTile = Instantiate(TileMapper[(TerrainTypes)TerrainTypeMap[i,j]]) as GameObject;
                SetPosition(newTile, i, 0, j);
            }
        }


        GameObject boss = Instantiate(turretBoss);
        SetPosition(boss, bossX, 0, bossZ);

        List<Vector2Int> nbhd = GetAllNeighbors(new Vector2Int(bossX, bossZ));

        TerrainTypeMap[bossX, bossZ] = (int)TerrainTypes.boss;
        foreach (var pos in nbhd) {
            TerrainTypeMap[pos.x, pos.y] = (int)TerrainTypes.boss;
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

        
        return (int)TerrainTypes.path;
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

            List<Vector2Int> path = FindPath(TerrainTypeMap, start, goal, 0, numberTilesX-1, 0, numberTilesZ-1);

            //Debug.Log(path.Count);
            foreach (Vector2Int wayPoint in path)
            {
                GameObject newTile = Instantiate(pathOutline) as GameObject;
                SetPosition(newTile, wayPoint.x, 0, wayPoint.y);

                _oldPathIndicators.Add(newTile);
            }

            currentpath = path;
        }

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (currentpath.Count > 0)
                isMoving = true;
        }
    }

    List<Vector2Int> GetWalkableNeighbors(Vector2Int hex)
    {
        List<Vector2Int> allNeighbors = GetAllNeighbors(hex);
        List<Vector2Int> neighbors = new List<Vector2Int>();

        foreach (Vector2Int neighbor in allNeighbors)
        {
            if (IsWalkable(GetFloorType(TerrainTypeMap, neighbor)))
                neighbors.Add(neighbor);

        }

        return neighbors;
    }

    void VisualizeNeighbors()
    {
        // Show the potential path
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            Vector2Int tile = GetTileIndexFromObject(hit.transform);

            foreach (Vector2Int neighbor in GetWalkableNeighbors(tile))
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
