using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossSceneGen : MonoBehaviour
{

    public GameObject dirtTile;
    public GameObject waterTile;
    public GameObject grassTile;

    private List<GameObject> Floor = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateFloorTiles();
    }

    // Update is called once per frame
    void Update()
    {
        FindTile();
    }

    void GenerateFloorTiles()
    {
        for (int j = 0; j < 20; j++)
        {
            for (int i = 0; i < 20; i++)
            {
                GameObject newTile;
                if (Random.Range(0, 10) < 7)
                    newTile = Instantiate(grassTile) as GameObject;
                else
                    newTile = Instantiate(waterTile) as GameObject;

                float iVal = i;
                if (j % 2 == 1)
                    iVal += 0.5f;

                newTile.transform.position = new Vector3(iVal, 0, j * Mathf.Sqrt(3) / 2);
                newTile.transform.Rotate(90, 0, 0);

                Floor.Add(newTile);
            }
        }
    }

    void FindTile()
    {
        int layerMask = 8;

        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Debug.Log("Did Hit");
            }
        }
    }
}
