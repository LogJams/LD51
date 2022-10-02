using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Main : MonoBehaviour {

    public Image fader;
    public float fadeTime = 1.0f;

    // Start is called before the first frame update
    void Awake() {
        StartCoroutine(FadeIn(fadeTime));
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
