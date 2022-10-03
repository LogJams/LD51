using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class BattleTimeManager : MonoBehaviour {

    public static BattleTimeManager instance;

    public event EventHandler OnTimerStart;
    public event EventHandler OnTimerEnd;

    //    public EventHandler OnBattleStartBoss1;
    //    public EventHandler OnBattleStartBoss2;

    public EventHandler OnBattleStart;
    public EventHandler OnBattleEnd;

    TurretBoss boss1;
    HuntingBoss boss2;
    


    bool inBattleMode = false;


    float time = 10f; //every 10 seconds!
    bool playerTurn = false;

    public bool PlayerActing() {
        return playerTurn || !inBattleMode;
    }

    public float SecondsRemaining() {
        return time;
    }

    public void SetBoss1(TurretBoss boss) {
        boss1 = boss;
        boss.OnDeath += EndBattle;
    }
    public void SetBoss2(HuntingBoss boss) {
        boss2 = boss;
        boss.OnDeath += EndBattle;
    }

    public void StartBoss1Battle() {
        inBattleMode = true;
        boss1.Awaken();
        OnBattleStart?.Invoke(this, EventArgs.Empty);
    }

    public void StartBoss2Battle() {
        inBattleMode = true;
        boss2.Awaken();
        OnBattleStart?.Invoke(this, EventArgs.Empty);
    }

    public void EndBattle(System.Object src, EventArgs e) {
        OnBattleEnd?.Invoke(this, EventArgs.Empty);
        inBattleMode = false;
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

        if (!inBattleMode) return;


        if (playerTurn) {
            time -= Time.deltaTime;
            if (time <= 0) {
                TimeOver();
            }
        }

    }
}
