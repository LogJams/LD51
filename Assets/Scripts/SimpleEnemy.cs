using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class SimpleEnemy : MonoBehaviour, Enemy {


    public event EventHandler OnDeath;

    public float moveTime = 2.0f;
    public float moveSpeed = 1.5f;


    Vector3 targetPos;
    float timer;

    public Zone parentZone;

    // Start is called before the first frame update
    void Start() {
        targetPos = transform.position;
    }

    // Update is called once per frame
    void Update() {

        timer -= Time.deltaTime;

        //pick a new target nearby
        if (timer <= 0) {
            timer = moveTime;
            List<Vector2Int> neighbors = OverworldManager.instance.GetBoundedNeighbors(this.transform, parentZone);
            if (neighbors.Count > 0) {
                Vector2Int next = neighbors[UnityEngine.Random.Range(0, neighbors.Count)];
                targetPos = HexagonHelpers.GetPositionFromIndex(next);
            }
        }

        //move toward the target and stand there
        Vector3 dp = targetPos - transform.position;
        dp.y = 0;
        if (dp.sqrMagnitude > 0.01f) {
            float speed = Mathf.Min(moveSpeed * Time.deltaTime, dp.magnitude);
            transform.position = transform.position + dp.normalized * speed;
        }
    }


    public bool OnPitfallTrap() {
        Die();
        return true;
    }


    private void Die() {
        //do some animation or particles or something
        OnDeath?.Invoke(this, EventArgs.Empty);
        Destroy(this.gameObject);
    }


}
