using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : MonoBehaviour, Interactable {

    public void Interact(TMP_Player src) {
        Debug.Log("Interacting with lootable thing!");
    }

}
