using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Point {
    public int x;
    public int y;
}


public class OverworldManager : MonoBehaviour {
    //parameters for hexagon geometry
    public int WORLD_WIDTH = 25;
    public int WORLD_HEIGHT = 25;
    float Y_SPACING = 0.866024540378f; //sqrt(3)/2



    //define the fixed locations here, then we WFC the rest
    public List<GameObject> terrainHexes;

    public List<Point> areas;
    
    //store the map and metadata about terrain generation here
    int[,] map;


    int AREA_SIZE = 3; // NxN area of 'ground' tiles where points of interest go

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

        float t0 = Time.realtimeSinceStartup;

        Debug.Log("Generate!");

        //generate flat areas + areas of interest
        foreach (var a0 in areas) {
            for (int i = 0; i < AREA_SIZE; i++) {
                for (int j = 0; j < AREA_SIZE; j++) {
                    map[i + a0.x, j + a0.y] = 0;
                }
            }
        }


        //run WFC on remaining tiles
        for (int i = 0; i < WORLD_WIDTH; i++) {
            for (int j = 0; j < WORLD_HEIGHT; j++) {
                if (map[i, j] == -1) {
                    map[i, j] = Random.Range(0, terrainHexes.Count);
                }
            }
        }

        Debug.Log("Generation complete @ " + (Time.realtimeSinceStartup - t0)*1000 + " ms" );

        SpawnTerrain();
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
