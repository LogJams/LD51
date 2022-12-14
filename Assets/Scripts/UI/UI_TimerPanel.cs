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

    BattleTimeManager timer;

    RectTransform rect;

    public void Start() {
        timer = BattleTimeManager.instance;
        button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "FIGHT";
        button.interactable = false;

        timer.OnTimerStart += StartOfRound;
        timer.OnTimerEnd += EndOfRound;
        timer.OnBattleStart += OnFightStart;
        timer.OnBattleEnd += OnFightEnd;

        rect = GetComponent<RectTransform>();

        rect.anchoredPosition = new Vector3(0, rect.rect.height + 1, 0);
    }


    public void OnFightStart(System.Object src, EventArgs e) {
        button.interactable = true;
        rect.anchoredPosition = new Vector3(0, 0, 0);
    }

    public void OnFightEnd(System.Object src, EventArgs e) {
        button.interactable = false;
        rect.anchoredPosition = new Vector3(0, rect.rect.height + 1, 0);
    }

    public void OnButtonClick() {
        timer.OnButtonClick();
    }

    public void EndOfRound(System.Object src, EventArgs e) {
        button.interactable = true;
    }

    public void StartOfRound(System.Object src, EventArgs e) {
        button.interactable = false;
    }


    void Update() {
        float time = timer.SecondsRemaining();
        text.text = "0:0" + Mathf.RoundToInt(time);
        slider.value = Mathf.Max(time / 10f, 0.000001f);
    }



}
