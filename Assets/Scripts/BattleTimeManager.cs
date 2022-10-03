using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class BattleTimeManager : MonoBehaviour {

    public static BattleTimeManager instance;

    public event EventHandler OnTimerStart;
    public event EventHandler OnTimerEnd;

    public EventHandler OnBattleStart;
    public EventHandler OnBattleEnd;

    TurretBoss boss1;
    HuntingBoss boss2;

    public bool BOSS_2_FIGHT = false;

    bool inBattleMode = false;


    float time = 10f; //every 10 seconds!
    bool playerTurn = false;

    public bool PlayerActing() {
        return playerTurn || !inBattleMode;
    }

    public bool InBattle() {
        return inBattleMode;
    }

    public float SecondsRemaining() {
        return time;
    }

    public void SetBoss1(TurretBoss boss) {
        boss1 = boss;
        boss.OnDeath += Boss1Killed;
        boss.OnDeath += EndBattle;
    }
    public void SetBoss2(HuntingBoss boss) {
        boss2 = boss;
        boss.OnDeath += Boss2Killed;
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
        BOSS_2_FIGHT = true; //this is the final fight
    }

    public void Boss1Killed(System.Object src, EventArgs e) {
        OverworldManager.instance.BossKilled(1);
    }
    public void Boss2Killed(System.Object src, EventArgs e) {
        OverworldManager.instance.BossKilled(2);

    }

    public void EndBattle(System.Object src, EventArgs e) {
        Debug.Log("End of the battle!!!");
        OnBattleEnd?.Invoke(this, EventArgs.Empty);
        inBattleMode = false;
        playerTurn = false;
        time = 10;
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
        OverworldManager.instance.ResetPlayerMovement();
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
