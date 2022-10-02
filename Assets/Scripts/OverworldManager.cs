using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static TerrainHelpers;
using static HexagonHelpers;

//spawn is where you start, boss is the location of the bosses, enemy is enemies+treasure, and friendly is friends
public enum ZONE_TYPES {
    NONE, SPAWN, BOSS, ENEMY, FRIENDLY
}

public class Zone {
    public int x, y, w, h;
    public ZONE_TYPES type;
    public int degree;
    public List<GameObject> decorations;
}

public class OverworldManager : MonoBehaviour {
    //parameters for hexagon geometry
    public int WORLD_WIDTH = 70;
    public int WORLD_HEIGHT = 70;

    public int NUM_ZONES = 10;
    public int AREA_SIZE = 3;
    public int AREA_SPACING = 1;

    public int CHUNK_SIZE = 15;

    int NUM_BOSS_ZONES = 3;

    //define the fixed locations here, then we WFC the rest
    public List<GameObject> terrainHexes;
    float Y_SPACING = 0.866024540378f; //sqrt(3)/2

    // Path outline
    public GameObject pathOutline;

    List<Zone> areas;

    //store the map and metadata about terrain generation here
    int[,] TerrainTypeMap;
    int[,] connectivity;

    GameObject[,] chunks;

    GameObject player;
    private PlayerManager playerManager;

    private List<Vector2Int> currentpath = new List<Vector2Int>();
    private List<GameObject> pathIndicators = new List<GameObject>();

    void Awake() {
        Generate();
        player = GameObject.FindGameObjectWithTag("Player");
        playerManager = player.GetComponent<PlayerManager>();
    }

    private void Update() {
        //only enable chunks near the player
        int pcx = (int) (player.transform.position.x / CHUNK_SIZE);
        int pcy = (int) (player.transform.position.z / Y_SPACING / CHUNK_SIZE);

        for (int i = 0; i < chunks.GetLength(0); i++) {
            for (int j = 0; j < chunks.GetLength(1); j++) {

                bool visible = (Mathf.Abs(i - pcx) <= 1) && (Mathf.Abs(j-pcy) <= 1);

                chunks[i, j].SetActive( visible );
            }
        }

        // Do path stuff
        if (!playerManager.isMoving)
        {
            ClearPath();
            ShowPath();
        }
        else
        {
            if (Input.GetMouseButtonUp(1))
            {
                if (currentpath.Count > 0 && playerManager.isMoving)
                    EndTurn();
            }

            playerManager.MovePlayer(currentpath, pathIndicators);
        }
    }

    void ClearPath()
    {
        foreach (GameObject obj in pathIndicators)
            Destroy(obj);

        pathIndicators.Clear();
    }

    void Cleanup() {

        //destroy all children (hexagons)
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        //create TerrainTypeMap and initialize to -1 (undefined)
        TerrainTypeMap = new int[WORLD_WIDTH, WORLD_HEIGHT];
        for (int i = 0; i < WORLD_WIDTH; i++) {
            for (int j = 0; j < WORLD_HEIGHT; j++) {
                TerrainTypeMap[i, j] = (int)TerrainHelpers.TerrainTypes.undefined;
            }
        }

        int cx = Mathf.CeilToInt(WORLD_WIDTH / (float)CHUNK_SIZE);
        int cy = Mathf.CeilToInt(WORLD_HEIGHT / (float)CHUNK_SIZE);
        chunks = new GameObject[cx, cy];

        for (int i = 0; i < cx; i++) {
            for (int j = 0; j < cy; j++) {
                chunks[i, j] = new GameObject("Chunk " + i + ", " + j);
                chunks[i, j].transform.parent = this.transform;
            }
        }
    }

    public void Generate() {

        Cleanup();

        //redo generation if we fail to place a zone
        bool failed = true;
        float t0 = Time.realtimeSinceStartup;


        while (failed) {
            failed = false;
            if (Time.realtimeSinceStartup - t0 > 10) {
                Debug.LogError("Warning! Generation timeout!");
                return;
            }

            areas = new List<Zone>();

            //randomly spawn N zones of interest
            for (int i = 0; i < NUM_ZONES; i++) {
                Zone newZone = new Zone();
                newZone.w = AREA_SIZE; newZone.h = AREA_SIZE;

                //make sure the zones don't overlap
                int x = Random.Range(1, WORLD_WIDTH - newZone.w - 1);
                int y = Random.Range(1, WORLD_WIDTH - newZone.h - 1);
                int numIter = 0;
                while (OverlapCheck(x, y, newZone.w, newZone.h) && numIter < 1000) {
                    x = Random.Range(1, WORLD_WIDTH - newZone.w - 1);
                    y = Random.Range(1, WORLD_WIDTH - newZone.h - 1);
                    numIter++;
                }

                //restart generation if we can't place a block after 1,000 tries
                if (numIter == 1000) {
                    //todo: restart generation
                    Debug.LogError("Warning! Could not place zone " + i + "!");
                    failed = true;
                    break;
                } else {
                    newZone.x = x; newZone.y = y;
                    areas.Add(newZone);
                }
            }
        }

        //create connectivity matrix
        float[,] distance = new float[areas.Count, areas.Count];
        connectivity = new int[areas.Count, areas.Count];
        for (int i = 0; i < areas.Count; i++) {
            for (int j = 0; j < areas.Count; j++) {
                distance[i, j] = 0;
                connectivity[i, j] = 0;
            }
        }

        //generate roads -- first connect each zone to its closest neighbor
        for (int i = 0; i < areas.Count; i++) {
            int x0 = areas[i].x + areas[i].w / 2;
            int y0 = areas[i].y + areas[i].h / 2;
            for (int j = i; j < areas.Count; j++) {
                int xf = areas[j].x + areas[j].w / 2;
                int yf = areas[j].y + areas[j].h / 2;
                //create the distance matrix
                float dist = (xf - x0) * (xf - x0) + (yf - y0) * (yf - y0);
                distance[i, j] = dist;
                distance[j, i] = dist;
            }
        }

        //create connectivity matrix based on distance matrix
        for (int i = 0; i < areas.Count; i++) {
            float minDist = Mathf.Infinity;
            int minIdx = -1;
            for (int j = 0; j < areas.Count; j++) {
                if (i == j) continue;

                if (distance[i, j] < minDist) {
                    minDist = distance[i, j];
                    minIdx = j;
                }
            }
            //connect them!
            connectivity[i, minIdx] = 1;
            connectivity[minIdx, i] = 1;
        }

        //check that connectivity matrtix is ergodic + add more edges
        Queue<int> connectQueue = new Queue<int>();
        List<int> connected = new List<int>();
        List<int> disconnected = new List<int>();

        connectQueue.Enqueue(0);

        while (connected.Count < areas.Count) {

            while (connectQueue.Count > 0) {
                //get the next index to check and add it to our list of connected items
                int idx = connectQueue.Dequeue();
                connected.Add(idx);

                //check connectivity matrix
                for (int i = 0; i < areas.Count; i++) {
                    if (i == idx) continue;
                    if (connectivity[idx, i] != 0 && !connected.Contains(i)) {
                        connectQueue.Enqueue(i);
                    }
                }
            }

            // get list of disconnected areas
            disconnected.Clear();
            for (int i = 0; i < areas.Count; i++) {
                if (!connected.Contains(i)) {
                    disconnected.Add(i);
                }
            }
            //break out if there are none
            if (disconnected.Count == 0) {
                break;
            }

            // get the disconnected component closest to any connected component and connect them!
            float minDist = Mathf.Infinity;
            int ic = -1; //connected index
            int id = -1; //disconnected index
            for (int i = 0; i < connected.Count; i++) {
                for (int j = 0; j < disconnected.Count; j++) {
                    int ii = connected[i];
                    int jj = disconnected[j];

                    if (distance[ii, jj] < minDist) {
                        minDist = distance[ii, jj];
                        ic = ii;
                        id = jj;
                    }
                }
            }

            //connect the two closest disconnected nodes
            connectivity[ic, id] = 1;
            connectivity[id, ic] = 1;
            connectQueue.Enqueue(id);
        }

        //configure zones to select their types, then decorate them
        ConfigureZones(connectivity);
        ZoneDecorator zoneDecorator = GetComponent<ZoneDecorator>();
        for (int i = 0; i < areas.Count; i++) {
            zoneDecorator.DecorateZone(areas[i]);
        }

        //set connections in the TerrainTypeMap data
        for (int i = 0; i < areas.Count; i++) {
            for (int j = i + 1; j < areas.Count; j++) {
                if (connectivity[i, j] == 0) continue;

                int x0 = areas[i].x + areas[i].w / 2;
                int xf = areas[j].x + areas[j].w / 2;

                int y0 = areas[i].y + areas[i].h / 2;
                int yf = areas[j].y + areas[j].h / 2;

                //flip which x is the origin so we just go from left to right
                int dx = xf - x0;
                int dy = yf - y0;

                //loop through x or y 
                if (Mathf.Abs(dx) > Mathf.Abs(dy)) {
                    for (int x = 0; x != dx; x += (int)Mathf.Sign(dx)) {
                        int xi = x0 + x;
                        int yi = y0 + Mathf.RoundToInt(((float)dy / dx) * x);
                        //set this and adjacent paths to dirt
                        TerrainTypeMap[xi, yi] = TerrainTypeMap[xi, yi - 1] = TerrainTypeMap[xi, yi + 1] = Random.value > 0.7f ? (int)TerrainTypes.path : (int)TerrainTypes.grass;
                    }
                } else {
                    for (int y = 0; y != dy; y += (int)Mathf.Sign(dy)) {
                        int yi = y0 + y;
                        int xi = x0 + Mathf.RoundToInt(((float)dx / dy) * y);
                        //set this and adjacent paths to dirt
                        TerrainTypeMap[xi, yi] = TerrainTypeMap[xi - 1, yi] = TerrainTypeMap[xi + 1, yi] = Random.value > 0.7f ? (int)TerrainTypes.path : (int)TerrainTypes.grass;
                    }
                }
            }
        }

        //generate zone ground, overwrite connections where necessary
        foreach (var zone in areas) {
            for (int i = 0; i < zone.w; i++) {
                for (int j = 0; j < zone.h; j++) {
                    TerrainTypeMap[i + zone.x, j + zone.y] = Random.value > 0.4f ? (int)TerrainTypes.path : (int)TerrainTypes.grass;
                }
            }
        }


        //generate a border around the entire TerrainTypeMap
        for (int i = 0; i < WORLD_WIDTH; i++) {
            TerrainTypeMap[i, 0] = TerrainTypeMap[i, WORLD_WIDTH - 1] = (int)TerrainTypes.cliff;
        }
        for (int j = 0; j < WORLD_HEIGHT; j++) {
            TerrainTypeMap[0, j] = TerrainTypeMap[WORLD_WIDTH - 1, j] = (int)TerrainTypes.cliff;
        }

        //generate remaining tiles
        for (int i = 1; i < WORLD_WIDTH - 1; i++) {
            for (int j = 1; j < WORLD_HEIGHT - 1; j++) {
                if (TerrainTypeMap[i, j] == (int)TerrainTypes.undefined) {
                    TerrainTypeMap[i, j] = (int)TerrainTypes.cliff;
                }
            }
        }

        //decorate the rest of the world

        Debug.Log("Generation complete @ " + (Time.realtimeSinceStartup - t0) * 1000 + " ms");

        SpawnTerrain();

        Debug.Log("Terrain spawned at " + (Time.realtimeSinceStartup - t0) * 1000 + " ms");

    }

    //return true if there's an overlap
    bool OverlapCheck(int x, int y, int w, int h) {
        for (int i = x; i < x + w; i++) {
            for (int j = y; j < y + h; j++) {
                for (int k = 0; k < areas.Count; k++) {
                    Zone z0 = areas[k];
                    if (z0.x - AREA_SPACING <= i && z0.x + z0.w + AREA_SPACING >= i && z0.y - AREA_SPACING <= j && z0.y + z0.h + AREA_SPACING >= j) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void ConfigureZones(int[,] adjacency) {
        //keep track of the degrees for initial assignment
        int minDegree = areas.Count + 1;
        int minDegreeIdx = -1;
        int maxDegree = 0;
        int maxDegreeIdx = -1;
        List<int> unassignedAreas = new List<int>();

        for (int i = 0; i < areas.Count; i++) {
            areas[i].degree = 0;
            unassignedAreas.Add(i);
            for (int j = 0; j < areas.Count; j++) {
                areas[i].degree = areas[i].degree + adjacency[i, j];
                areas[i].type = ZONE_TYPES.NONE;
            }
            if (areas[i].degree < minDegree) {
                minDegree = areas[i].degree;
                minDegreeIdx = i;
            }
            if (areas[i].degree > maxDegree) {
                maxDegree = areas[i].degree;
                maxDegreeIdx = i;
            }
        }


        //the zone with the lowest degree is the player
        areas[minDegreeIdx].type = ZONE_TYPES.SPAWN;
        //the zone with the highest degree is friendly
        areas[maxDegreeIdx].type = ZONE_TYPES.FRIENDLY;
        //remove them from unassigned
        unassignedAreas.Remove(minDegreeIdx);
        unassignedAreas.Remove(maxDegreeIdx);

        //assign X fight rooms (we need to design this)
        for (int i = 0; i < NUM_BOSS_ZONES; i++) {
            int idx = unassignedAreas[Random.Range(0, unassignedAreas.Count)];
            unassignedAreas.Remove(idx);
            areas[idx].type = ZONE_TYPES.BOSS;
        }
        //remaining rooms will be enemies
        for (int i = 0; i < unassignedAreas.Count; i++) {
            areas[unassignedAreas[i]].type = ZONE_TYPES.ENEMY;
        }
        unassignedAreas.Clear();
    }

    public void SpawnTerrain() {

        for (uint i = 0; i < WORLD_WIDTH; i++) {
            for (uint j = 0; j < WORLD_HEIGHT; j++) {
                //set the hexagon's position
                float dx = 0;
                if (j % 2 == 1) {
                    dx = 0.5f;
                }
                Vector3 pos = new Vector3(i + dx, 0, j * Y_SPACING);

                //spawn the correct hex type

                int cx = (int) (i / CHUNK_SIZE);
                int cy = (int) (j / CHUNK_SIZE);

                Instantiate(terrainHexes[TerrainTypeMap[i, j]], pos, Quaternion.identity, chunks[cx,cy].transform);

            }
        }
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

            List<Vector2Int> path = FindPath(TerrainTypeMap, start, goal, 0, WORLD_HEIGHT - 1, 0, WORLD_WIDTH - 1);

            foreach (Vector2Int wayPoint in path)
            {
                GameObject newTile = Instantiate(pathOutline) as GameObject;
                SetPosition(newTile, wayPoint.x, 0, wayPoint.y);

                pathIndicators.Add(newTile);
            }

            currentpath = path;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentpath.Count > 0 && !playerManager.isMoving)
                playerManager.isMoving = true;
        }
    }

    public void EndTurn()
    {
        if (currentpath.Count > 0)
        {
            // Clear path except for current goal
            Vector2Int goalPos = currentpath[currentpath.Count - 1];
            currentpath.Clear();
            currentpath.Add(goalPos);
            
            for (int i = pathIndicators.Count - 1; i >= 0; i--)
                Destroy(pathIndicators[i]);
        
            pathIndicators.Clear();
        }
    }
}
