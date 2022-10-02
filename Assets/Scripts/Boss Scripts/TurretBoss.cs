using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBoss : MonoBehaviour {

    public Transform turret;

    public Transform playerTransform;


    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {

        Vector3 lookDir = playerTransform.position - transform.position;
        lookDir.y = 0;

        turret.LookAt(lookDir);

    }
}
