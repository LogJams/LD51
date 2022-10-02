using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class Teleport : MonoBehaviour, Interactable {

    public int sceneIndex = -1;

    public void Interact(TMP_Player src) {
        //disable the player
        GameObject.FindGameObjectWithTag("Player").GetComponent<TMP_Player>().paused = true;

        //have the UI fade out before loading the level
        StartCoroutine(LoadCoroutine());
    }


    IEnumerator LoadCoroutine() {
        UI_Main ui = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UI_Main>();

        float fadeTime = ui.fadeTime;

        StartCoroutine(ui.FadeOut(fadeTime));
        yield return new WaitForSeconds(fadeTime);
        
        SceneManager.LoadScene(sceneIndex);
        yield return null;
    }

}
