using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this script unlocks the pitfall ability when clicked on!
/// </summary>
public class UnlockPitfall : MonoBehaviour, Interactable {


    void Update() {

        transform.position = new Vector3(transform.position.x, 1 + 0.25f*Mathf.Cos(Time.time / 3.0f), transform.position.z);

    }

    public void Interact(PlayerManager src) {

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().UnlockPitfall();

        Destroy(this.gameObject);
    }

}
