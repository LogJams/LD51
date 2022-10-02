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

    private PlayerManager playerManager;
    private GameObject player;
    private CharacterController controller;

    private List<Vector2Int> currentpath;

    private static int numberTilesX = 20;
    private static int numberTilesZ = 20;

    private List<GameObject> pathIndicators = new List<GameObject>();

    private Dictionary<TerrainTypes, GameObject> TileMapper;

    private int[,] TerrainTypeMap = new int[numberTilesX,numberTilesZ];

    private void Awake() {
        GeneratePlayer();
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateTileDictionary();
        GenerateFloorTiles();
        timer.OnTimerEnd += EndTurn;
    }

    // Update is called once per frame
    void Update()
    {

        if (!playerManager.isMoving)
        {
            //do nothing if we are mousing over the UI or it's not the player's turn
            if (!EventSystem.current.IsPointerOverGameObject() && timer.PlayerCanAct()) {
                ClearPath();
                ShowPath();
            }
        } else
        {
            playerManager.MovePlayer(currentpath, pathIndicators);
        }
    }


    public void EndTurn(System.Object src, EventArgs e) {
        Vector2Int goalPos = currentpath[currentpath.Count - 1];
        currentpath.Clear();

        currentpath.Add(goalPos);

        for (int i = pathIndicators.Count-1; i >= 0; i--) {
            Destroy(pathIndicators[i]);
        }
        pathIndicators.Clear();

    } 


    void ClearPath()
    {
        foreach (GameObject obj in pathIndicators)
            Destroy(obj);

        pathIndicators.Clear();
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

        boss.GetComponent<TurretBoss>().playerTransform = player.transform;

        List<Vector2Int> nbhd = GetAllNeighbors(new Vector2Int(bossX, bossZ));

        TerrainTypeMap[bossX, bossZ] = (int)TerrainTypes.boss;
        foreach (var pos in nbhd) {
            TerrainTypeMap[pos.x, pos.y] = (int)TerrainTypes.boss;
        }

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
        playerManager = new PlayerManager(player); 
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

                pathIndicators.Add(newTile);
            }

            currentpath = path;
        }

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (currentpath.Count > 0)
                playerManager.isMoving = true;
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

            foreach (Vector2Int neighbor in GetWalkableNeighbors(TerrainTypeMap, tile, 0, numberTilesX-1, 0, numberTilesZ-1))
            {
                GameObject newTile = Instantiate(pathOutline) as GameObject;
                SetPosition(newTile, neighbor.x, 0, neighbor.y);

                pathIndicators.Add(newTile);
            }
        }
    }

}
