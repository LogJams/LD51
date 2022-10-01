using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OverworldManager : MonoBehaviour {

    public int WORLD_WIDTH = 20;
    public int WORLD_HEIGHT = 20;
    float Y_SPACING = 0.866024540378f; //sqrt(3)/2



    //define the fixed locations here, then we WFC the rest
    public List<GameObject> terrainHexes;

    

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
        foreach (Transform child in transform) {
            DestroyImmediate(child.gameObject);
        }

        map = new int[WORLD_WIDTH,WORLD_HEIGHT];

    }

    public void Generate() {
        Cleanup();

     

        Debug.Log("Generate!");

        Quaternion rot = Quaternion.identity;
        

        for (uint i = 0; i < WORLD_WIDTH; i++) {
            for (uint j = 0; j < WORLD_HEIGHT; j++) {
                map[i, j] = Random.Range(0, terrainHexes.Count);

                //set the hexagon's position
                float dx = 0;
                if (j % 2 == 0) {
                    dx = 0.5f;
                }
                Vector3 pos = new Vector3(i + dx, 0, j * Y_SPACING);

                //show the terrain
                Instantiate(terrainHexes[ map[i,j] ], pos, rot, this.transform);

            }
        }


    }





    }
