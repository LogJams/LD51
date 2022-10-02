using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using System;

public class UI_TimerPanel : MonoBehaviour {

    public Button button;
    public Slider slider;
    public TMPro.TextMeshProUGUI text;

    public event EventHandler OnTimerEnd;

    float time = 10f; //every 10 seconds!
    
    bool playerTurn = false;

    public bool PlayerCanAct() {
        return playerTurn;
    }

    public void Update() {

        if (playerTurn) UpdateTimer();
    

    }


    public void OnButtonClick() {
        if (!playerTurn) {
            StartTurn();
        }
        else {
            TimeOver();
        }
    }

    void StartTurn() {
        time = 10f;
        playerTurn = true;
        button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "END";
    }


    void UpdateTimer() {
        time -= Time.deltaTime;

        text.text = "0:0" + Mathf.RoundToInt(time);
        slider.value = time / 10f;

        if (time <= 0) {
            text.text = "0:10";
            slider.value = 1;
            TimeOver();
        }

    }

    void TimeOver() {
        button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "START";
        playerTurn = false;
        OnTimerEnd?.Invoke(this, EventArgs.Empty);
    }

}
