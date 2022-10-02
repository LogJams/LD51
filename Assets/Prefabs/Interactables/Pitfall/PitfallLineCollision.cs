using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PitfallLineCollision : MonoBehaviour
{

    public PitfallManager parent;
    BoxCollider col ;


    private void Awake() {
        col = GetComponentInChildren<BoxCollider>();
        col.enabled = false;
    }

    // Start is called before the first frame update
    public void Setup() {
        LineRenderer line = GetComponent<LineRenderer>();
        Vector3 startPos = line.GetPosition(0);
        Vector3 endPos = line.GetPosition(line.positionCount - 1);

        float thickness = line.endWidth;
        float lineLength = Vector3.Distance(startPos, endPos);

        col.size = new Vector3(thickness, thickness, lineLength);
        col.transform.position = (startPos + endPos) / 2;

        Quaternion rot = Quaternion.LookRotation(endPos - startPos, Vector3.up);
        col.transform.rotation = rot;
        col.enabled = true;
    }

    private void OnTriggerEnter(Collider other) {
        //do pitfall manager things when triggered!
        if (!other.CompareTag("Player")) {
            parent.RecieveTrigger(other.gameObject);
        }
    }

}
