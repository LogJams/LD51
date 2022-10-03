using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UI_TitlePage : MonoBehaviour {

    public Transform creditText;

    Vector3 p0;

    public float a0 = 0.3f;
    public float a1 = 0.5f;
    public float w0 = 0.3f;
    public float w1 = 0.4f;

    public void StartButton() {
        SceneManager.LoadScene(1);
    }


    public void QuitButton() {
        Application.Quit();
    }

    private void Start() {
        p0 = creditText.position;
    }

    private void Update() {
        creditText.position = p0 + new Vector3(a0*Mathf.Cos(w0*Time.time), a1*Mathf.Sin(w1*Time.time));
    }

}
