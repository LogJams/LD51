using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemRemovalScript : MonoBehaviour {

    public Enemy attachedEnemy;


    private void Awake() {
        Transform tf = transform;
        while (attachedEnemy == null) {
            attachedEnemy = tf.GetComponent<Enemy>();
            tf = tf.parent;
        }
    }


    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            attachedEnemy.LoseGem();
            Destroy(this.gameObject);
        }
    }


}
