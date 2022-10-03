using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class UI_Main : MonoBehaviour {

    public Image fader;
    public float fadeTime = 1.0f;

    public float winDelay = 2.0f;


    // Start is called before the first frame update
    void Awake() {
        StartCoroutine(FadeIn(fadeTime));
    }

    private void Start() {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().OnUnlockPitfall += PitfallUnlocked;

        BattleTimeManager.instance.OnBattleEnd += OnCombatFinished;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().OnDeath += OnPlayerDeath;
    }

    public void PitfallUnlocked(System.Object src, System.EventArgs e) {

        //todo: show some text or something and add pitfall to the UI

    }

    public void OnCombatFinished(System.Object src, System.EventArgs e) {
        if (BattleTimeManager.instance.BOSS_2_FIGHT) {
            //we defeated boss 2, fade out and load a new scene!
            StartCoroutine(FadeOut(winDelay));
            StartCoroutine(LoadScene(0, winDelay)); //main menu is scene 0 in build order
        }
    }

    public void OnPlayerDeath(System.Object src, System.EventArgs e) {
        //we died, restart!
        StartCoroutine(FadeOut(winDelay / 2));
        StartCoroutine(LoadScene(1, winDelay / 2)); // game is scene 1 in build order
    }


    IEnumerator LoadScene(int idx, float delayInSec) {
        yield return new WaitForSeconds(delayInSec);

        SceneManager.LoadScene(idx);
        yield return null;
    }

    // Update is called once per frame
    void Update() {
        

        if (Input.GetKeyDown(KeyCode.G)) {
            StartCoroutine(FadeOut(fadeTime));
        }
        if (Input.GetKeyDown(KeyCode.H)) {
            StartCoroutine(FadeIn(fadeTime));
        }

    }

    public IEnumerator FadeOut(float duration) {
        fader.color = new Color(0, 0, 0, 0);
        fader.enabled = true;

        float t0 = Time.time;
        while (Time.time - t0 < duration) {
            fader.color = Color.Lerp(new Color(0, 0, 0, 0), Color.black, Mathf.Pow((Time.time - t0) / (duration), 2) );

            yield return new WaitForEndOfFrame();

        }

        fader.color = Color.black;

        yield return null;
    }

    public IEnumerator FadeIn(float duration) {
        fader.color = Color.black;
        fader.enabled = true;
        float t0 = Time.time;


        while (Time.time - t0 < duration) {
            fader.color = Color.Lerp(Color.black, new Color(0, 0, 0, 0), Mathf.Pow( (Time.time - t0) / (duration), 2) );

            yield return new WaitForEndOfFrame();

        }

        fader.enabled = false;

        yield return null;
    }


}
