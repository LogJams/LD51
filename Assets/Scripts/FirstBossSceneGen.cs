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

    private BattleTimeManager timer;

    private PlayerManager playerManager;
    private GameObject player;

    private static int SCENE_WIDTH = 20;
    private static int SCENE_HEIGHT = 20;

    private List<GameObject> pathIndicators = new List<GameObject>();
    private List<Vector2Int> currentpath;

    private Dictionary<TerrainTypes, GameObject> TileMapper;

    private int[,] TerrainTypeMap = new int[SCENE_WIDTH,SCENE_HEIGHT];

    private void Awake() {
        GeneratePlayer();
    }

    // Start is called before the first frame update
    void Start()
    {
        timer = BattleTimeManager.instance;
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
            if (!EventSystem.current.IsPointerOverGameObject() && timer.PlayerActing()) {
                ClearPath();
                ShowPath();
            }
        } else
        {
            if (Input.GetMouseButtonUp(1))
            {
                if (currentpath.Count > 0 && playerManager.isMoving)
                    EndTurn();
            }

            playerManager.MovePlayer(currentpath, pathIndicators);
        }
    }

    public void EndTurn(System.Object src, EventArgs e) {
        EndTurn();
    }

    public void EndTurn()
    {
        if (currentpath.Count > 1) {
            Vector2Int goalPos = currentpath[currentpath.Count - 1];
            currentpath.Clear();
            currentpath.Add(goalPos);
        }
        for (int i = pathIndicators.Count - 1; i >= 0; i--)
        {
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
        for (int i = 0; i < SCENE_WIDTH; i++) {
            TerrainTypeMap[i, 0] = (int)TerrainTypes.cliff;
            TerrainTypeMap[i, SCENE_HEIGHT-1] = (int)TerrainTypes.cliff;
        }
        for (int j = 0; j < SCENE_HEIGHT; j++) {
            TerrainTypeMap[0, j] = (int)TerrainTypes.cliff;
            TerrainTypeMap[SCENE_WIDTH - 1, j] = (int)TerrainTypes.cliff;
        }

        int bossX = SCENE_WIDTH / 2;
        int bossZ = SCENE_HEIGHT - 4;


        //fill in the inside
        for (int j = 1; j < SCENE_HEIGHT-1; j++) {
            for (int i = 1; i < SCENE_WIDTH-1; i++) {
                // Draw a random tile
                TerrainTypeMap[i, j] = GetRandomFloorType();
            }
        }

        //generate the final tilemap
        for (int j = 0; j < SCENE_HEIGHT; j++) {
            for (int i = 0; i < SCENE_WIDTH; i++) {
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

    private int GetRandomFloorType()
    {
        if (UnityEngine.Random.Range(0, 10) < 3)
            return (int)TerrainTypes.grass;

        
        return (int)TerrainTypes.path;
    }

    void GeneratePlayer()
    {
        player = Instantiate(playerPrefab) as GameObject;
        player.transform.position = new Vector3(SCENE_WIDTH/2.0f, 2, SCENE_HEIGHT*Mathf.Sqrt(3)/4);
        playerManager = player.GetComponent<PlayerManager>();
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

            List<Vector2Int> path = FindPath(TerrainTypeMap, start, goal, 0, SCENE_WIDTH-1, 0, SCENE_HEIGHT-1);

            //Debug.Log(path.Count);
            foreach (Vector2Int wayPoint in path)
            {
                GameObject newTile = Instantiate(pathOutline) as GameObject;
                SetPosition(newTile, wayPoint.x, 0, wayPoint.y);

                pathIndicators.Add(newTile);
            }

            currentpath = path;
        }

        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
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

            foreach (Vector2Int neighbor in GetWalkableNeighbors(TerrainTypeMap, tile, 0, SCENE_WIDTH-1, 0, SCENE_HEIGHT-1))
            {
                GameObject newTile = Instantiate(pathOutline) as GameObject;
                SetPosition(newTile, neighbor.x, 0, neighbor.y);

                pathIndicators.Add(newTile);
            }
        }
    }

}
