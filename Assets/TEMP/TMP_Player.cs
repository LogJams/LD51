using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class TMP_Player : MonoBehaviour {

    public float speed = 2.5f;

    public float interactionRange = 2.0f;

    public bool paused = false;

    public List<PlayerAbility> abilities;

    public event EventHandler OnGainAbility;

    // Start is called before the first frame update
    void Start() {
        abilities[0].unlocked = true;
        OnGainAbility?.Invoke(this, EventArgs.Empty);
    }

    // Update is called once per frame
    void Update() {
        if (paused) return;


        Movement();

        MouseInput();
    }



    void Movement() {
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("Vertical"), 0, -Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("Vertical"));
        GetComponent<CharacterController>().SimpleMove(movement.normalized * speed);
    }

    void MouseInput() {
        if (Input.GetMouseButtonUp(0)) {

            RaycastHit hitInfo;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo) ) {
                Interactable target = hitInfo.collider.gameObject.GetComponentInParent<Interactable>();
                Vector3 distance = hitInfo.collider.transform.position - transform.position;
                distance.y = 0;

                if (target != null && distance.sqrMagnitude <= interactionRange*interactionRange) {
                    //.Interact(this); broke this to fix game
                }
            }


        }
    }
}
