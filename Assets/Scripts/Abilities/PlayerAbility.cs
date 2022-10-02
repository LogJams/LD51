using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAbility", menuName = "ScriptableObjects/PlayerAbility", order = 1)]
public class PlayerAbility : ScriptableObject {

    public bool unlocked;

    //awake is called once on game start
    private void Awake() {
        unlocked = false;
        Debug.Log("Awakee");
    }

}
