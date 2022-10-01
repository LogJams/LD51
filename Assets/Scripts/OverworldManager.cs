using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldManager : MonoBehaviour {

    public int WORLD_WIDTH = 100;
    public int WORLD_HEIGHT = 100;

    //define the fixed locations here, then we WFC the rest

    public GameObject hexagon;

    private void Awake() {
        Generate();
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Generate() {
        Debug.Log("Generate!");

        for (uint i = 0; i < WORLD_WIDTH; i++) {
            for (uint j = 0; j < WORLD_WIDTH; j++) {

                Vector3 pos = new Vector3(i, 0, j);

                Instantiate(hexagon, pos, Quaternion.identity, this.transform);

            }
        }


    }





    }
