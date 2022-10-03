using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this script unlocks the pitfall ability when clicked on!
/// </summary>
public class HealthPickup : MonoBehaviour, Interactable {

    public Transform rotator;

    public float rotRate = 30f;

    void Update() {
        rotator.Rotate(Vector3.up, Time.deltaTime * rotRate);
    }

    public void Interact(PlayerManager src) {

        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().UnlockPitfall();

        //Destroy(this.gameObject);
    }

    public void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerManager>().Heal();
            Destroy(this.gameObject);
        }
    }

}
