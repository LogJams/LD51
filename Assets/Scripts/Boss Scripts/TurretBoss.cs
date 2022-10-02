using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBoss : MonoBehaviour {

    public Transform turret;

    Transform playerTransform;


    // Start is called before the first frame update
    void Start() {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update() {

        Vector3 lookDir = playerTransform.position - transform.position;
        lookDir.y = 0;

        turret.LookAt(lookDir);

    }
}
