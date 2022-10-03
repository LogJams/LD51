using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

using static HexagonHelpers;

public class HuntingBoss : MonoBehaviour, Enemy {

    public event EventHandler OnDeath;

    public GameObject tileOutline;

    Transform playerTransform;

    public List<ParticleSystem> fireParticles;
    public List<ParticleSystem> smokeParticles;

    public bool isAlive = true;

    //used for falling
    bool falling = false;
    float elapsed;
    Quaternion q0; //saved initial rotation
    Quaternion qf; //rotation on the ground

    bool awakened = false;
    private void Awake() {

    }

    // Start is called before the first frame update
    void Start() {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void LoseGem() {
        //we die when we lose the gem
        Die();
    }

    public void Awaken() {
        if (awakened) return;
        BattleTimeManager.instance.OnTimerStart += OnRoundStart;
        BattleTimeManager.instance.OnTimerEnd += OnRoundEnd;
        awakened = true;
    }

    public void Die() {
        isAlive = false; //so player doesn't take damage after death
        falling = false; //to stop update from updating + restarting the attack cycle
        StopAllCoroutines();
        BattleTimeManager.instance.OnTimerStart -= OnRoundStart;
        BattleTimeManager.instance.OnTimerEnd -= OnRoundEnd;

        foreach (var ps in fireParticles) {
            ps.Stop();
        }

        //play some death audio, maybe particle effects
        OnDeath?.Invoke(this, EventArgs.Empty);

    }

    public void OnRoundStart(System.Object src, EventArgs e) {
        StartCoroutine(AttackCoroutine());
    }

    public void OnRoundEnd(System.Object src, EventArgs e) {
        StopAllCoroutines();
    }

    public bool OnPitfallTrap()
    {
        //we wil set the falling flag to true and kill all coroutines
        falling = true;
        elapsed = 0;
        q0 = transform.rotation;
        qf = Quaternion.LookRotation(-Vector3.up, transform.transform.forward);
        StopAllCoroutines();
        return true;
    }




    void Update() {
        transform.position = new Vector3(transform.position.x, 1, transform.position.z); //keep it on the ground!!!

        if (!falling) return; //the update method handles falling and getting back up

        float fallTime = 0.5f;
        float downTime = 2.0f;
        float riseTime = 0.5f;

        if (elapsed < fallTime) {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(q0, qf, elapsed / fallTime);
        }
        else if (elapsed < fallTime + downTime) {
            transform.rotation = qf;
            //hunter stays on the ground
            if (BattleTimeManager.instance.PlayerActing()) {
                elapsed += Time.deltaTime; //only increment the clock when the action is going on
            }
            //maybe add some fun little wiggles/rotation to show the boss struggling

        }
        else if (elapsed < fallTime + downTime + riseTime) {
            elapsed += Time.deltaTime;
            //rise back up to q0 (we could do something more fun than a quatenrion lerp I guess)
            transform.rotation = Quaternion.Lerp(qf, q0, (elapsed - fallTime - downTime) / riseTime);
        }
        else {//we're up, back into the fight!
            transform.rotation = q0;
            falling = false;
            if (BattleTimeManager.instance.PlayerActing()) {
                StartCoroutine(AttackCoroutine());
            }
        }

    }

    public IEnumerator AttackCoroutine() {
        //todo: pick a random attack pattern (or cycle through them)
        float rotationTime = 1.0f;
        float moveSpeed = 5.0f;
        float waitTime = 1.0f;

        //we call StopAllCoroutines at the end of the round
        while (BattleTimeManager.instance.PlayerActing() && !falling) {
            //pick a straight line + look at the player & random offset
            Vector3 lookTarget = playerTransform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), 0, UnityEngine.Random.Range(-2f, 2f)) - transform.position;
            Vector3 runTarget = playerTransform.position;

            lookTarget.y = 0;
            StartCoroutine(LookCoroutine(lookTarget, rotationTime));

            //highlight the hexagons in a line -- get this from the attack pattern
            float x0 = transform.position.x;
            float z0 = transform.position.z;
            Quaternion qf = Quaternion.LookRotation(lookTarget.normalized, Vector3.up);

            // wait to rotate
            yield return new WaitForSeconds(rotationTime);

            // wait to run
            yield return new WaitForSeconds(waitTime);
            StartCoroutine(RunCoroutine(runTarget, moveSpeed));
            //***** attack now!
            //TODO: play running animation & sfx

            //TODO: damage the player if they are hit by the boss

            //***** clean up the attack
            // stop the particles and attack points


            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    public IEnumerator LookCoroutine(Vector3 lookDir, float duration) {
        Quaternion q0 = transform.rotation;
        Quaternion qf = Quaternion.LookRotation(lookDir, Vector3.up);
        float elapsed = 0;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(q0, qf, elapsed / duration);
            yield return new WaitForEndOfFrame();
        }

        transform.rotation = qf;

        yield return null;
    }

    public IEnumerator RunCoroutine(Vector3 runTarget, float speed)
    {
        float elapsed = 0;
        float duration = (runTarget - transform.position).magnitude/speed;
        Vector3 runDirection = (runTarget - transform.position).normalized;

        runDirection.y = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position += speed * runDirection * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.position = runTarget;

        yield return null;
    }

}
