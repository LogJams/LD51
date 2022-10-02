using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class BattleTimeManager : MonoBehaviour {

    public static BattleTimeManager instance;

    public event EventHandler OnTimerStart;
    public event EventHandler OnTimerEnd;

    float time = 10f; //every 10 seconds!
    bool playerTurn = false;

    public bool PlayerActing() {
        return playerTurn;
    }

    public float SecondsRemaining() {
        return time;
    }

    private void Awake() {
        //singleton pattern
        if (instance != null && instance != this) {
            Destroy(this);
            return;
        }
        instance = this;
    }


    public void OnButtonClick() {
        if (PlayerActing()) {
            //do nothing?
        }
        else {
            //start the timer!
            time = 10;
            playerTurn = true;
            OnTimerStart?.Invoke(this, EventArgs.Empty);
        }

        time = 10f;

    }

    void TimeOver() {
        time = 10f;
        playerTurn = false;
        OnTimerEnd?.Invoke(this, EventArgs.Empty);
    }


    // Update is called once per frame
    void Update() {
        
        if (playerTurn) {
            time -= Time.deltaTime;
            if (time <= 0) {
                TimeOver();
            }
        }

    }
}
