using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Zone {
    public int x, y, w, h;
}


public class OverworldManager : MonoBehaviour {
    //parameters for hexagon geometry
    public int WORLD_WIDTH = 70;
    public int WORLD_HEIGHT = 70;
    public int NUM_ZONES = 10;
    public int AREA_SIZE = 3;

    public int AREA_SPACING = 1;

    //define the fixed locations here, then we WFC the rest
    public List<GameObject> terrainHexes;
    float Y_SPACING = 0.866024540378f; //sqrt(3)/2


    List<Zone> areas;

    int[,] connectivity;
    
    //store the map and metadata about terrain generation here
    int[,] map;



    private void Awake() {
        Generate();
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    void Cleanup() {
        //destroy all children (hexagons)
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        //create map and initialize to -1 (undefined)
        map = new int[WORLD_WIDTH,WORLD_HEIGHT];
        for (int i = 0; i < WORLD_WIDTH; i++) {
            for (int j = 0; j < WORLD_HEIGHT; j++) {
                map[i, j] = -1;
            }
        }
    }

    public void Generate() {
        Cleanup();

        //redo generation if we fail to place a zone
        bool failed = true;

        while (failed) {
            failed = false;

            areas = new List<Zone>();

            //spawn the start zone
            Zone newZone = new Zone();
            newZone.x = 1; newZone.y = 1;
            newZone.w = AREA_SIZE; newZone.h = AREA_SIZE;
            areas.Add(newZone);

            //randomly spawn N zones of interest
            for (int i = 0; i < NUM_ZONES; i++) {
                newZone = new Zone();
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


        float t0 = Time.realtimeSinceStartup;

        Debug.Log("Generate!");

        //generate areas of interest
        foreach (var zone in areas) {
            for (int i = 0; i < zone.w; i++) {
                for (int j = 0; j < zone.h; j++) {
                    map[i + zone.x, j + zone.y] = 0;
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
            for (int j = 0; j < areas.Count; j++) {
                int xf = areas[j].x + areas[j].w / 2;
                int yf = areas[j].y + areas[j].h / 2;
                //create the distance matrix
                float dist = (xf - x0) * (xf - x0) + (yf - y0) * (yf - y0);
                distance[i, j] = dist;

                Debug.Log("i, j = " + i + ", " + j + "; dist = " + dist);

            }
        }

        //create connectivity matrix based on distance matrix



        //draw roads for each pair of connected areas
        for (int i = 0; i < areas.Count; i++) { //rows of connectivity matrix

        }





        //generate remaining tiles
        for (int i = 0; i < WORLD_WIDTH; i++) {
            for (int j = 0; j < WORLD_HEIGHT; j++) {
                if (map[i, j] == -1) {
                    map[i, j] = Random.Range(1, terrainHexes.Count);
                }
            }
        }

        Debug.Log("Generation complete @ " + (Time.realtimeSinceStartup - t0)*1000 + " ms" );

        SpawnTerrain();
    }

    //return true if there's an overlap
    bool OverlapCheck(int x, int y, int w, int h) {

        for (int i = x; i < x + w; i++) {
            for (int j = y; j < y+h; j++) {
                for (int k = 0; k < areas.Count; k++) {
                    Zone z0 = areas[k];
                    if (z0.x - AREA_SPACING <= i && z0.x + z0.w + AREA_SPACING >= i && z0.y - AREA_SPACING <= j && z0.y + z0.h + AREA_SPACING >= j ) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public void SpawnTerrain() {
        //todo: only loop around near the camera (?)
        for (uint i = 0; i < WORLD_WIDTH; i++) {
            for (uint j = 0; j < WORLD_HEIGHT; j++) {
                //set the hexagon's position
                float dx = 0;
                if (j % 2 == 0) {
                    dx = 0.5f;
                }
                Vector3 pos = new Vector3(i + dx, 0, j * Y_SPACING);

                //spawn the correct hex type
                Instantiate(terrainHexes[map[i, j]], pos, Quaternion.identity, this.transform);

            }
        }
    }





}
