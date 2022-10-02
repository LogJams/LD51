using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, Interactable {

    public Transform movingPart;
    Vector3 p0;


    public float openTime = 0.8f;

    public float closeAfter = 1.5f;

    bool changing = false;


    void Start() {
        p0 = movingPart.transform.position;
    }

    public void Interact(TMP_Player src) {
        Debug.Log("Interacting with a door to open/close it!");
        if (!changing) {
            StartCoroutine(OpenDoor());
        }
    }



    IEnumerator OpenDoor() {
        changing = true;
        float t0 = Time.time;
        float tf = t0 + openTime;

        while (Time.time < tf) {
            movingPart.position = Vector3.Lerp(p0, p0 + new Vector3(0, -1.1f, 0), (Time.time - t0) / (tf - t0));
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(closeAfter);

        t0 = Time.time;
        tf = t0 + openTime;
        while (Time.time < tf) {
            movingPart.position = Vector3.Lerp(p0 + new Vector3(0, -1.1f, 0), p0, (Time.time - t0) / (tf - t0));
            yield return new WaitForEndOfFrame();
        }

        changing = false;
        yield return null;
    }
    

}
