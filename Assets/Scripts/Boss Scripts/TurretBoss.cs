using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

using static HexagonHelpers;

public class TurretBoss : MonoBehaviour, Enemy {

    public event EventHandler OnDeath;

    public Transform turret;
    public GameObject tileOutline;

    public GameObject PitfallUnlocker;

    Transform playerTransform;


    public List<ParticleSystem> fireParticles;
    public List<ParticleSystem> smokeParticles;

    HashSet<Vector2Int> attackLocations;
    List<GameObject> tileOutlines;

    AudioSource audioSrc;

    bool awakened = false;


    private void Awake() {
        tileOutlines = new List<GameObject>();
        attackLocations = new HashSet<Vector2Int>();
    }
    // Start is called before the first frame update
    void Start() {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        audioSrc = GetComponent<AudioSource>();
    }

    public bool OnPitfallTrap() {
        //pitfall trap does nothing but destroy the trap
        return true;
    }

    //subscribe to the battle events!
    public void Awaken() {
        if (awakened) return;
        BattleTimeManager.instance.OnTimerStart += OnRoundStart;
        BattleTimeManager.instance.OnTimerEnd += OnRoundEnd;
        awakened = true;
    }

    public void Die() {
        BattleTimeManager.instance.OnTimerStart -= OnRoundStart;
        BattleTimeManager.instance.OnTimerEnd -= OnRoundEnd;

        StopAllCoroutines();
        Cleanup();

        OnDeath?.Invoke(this, EventArgs.Empty);

        Instantiate(PitfallUnlocker, transform.position + new Vector3(2, 2, 2), Quaternion.identity);

        //todo: requrest smoke particles from a ParticleManager
        //play death animation
        StartCoroutine(DeathCoroutine());
    }

    public void OnRoundStart(System.Object src, EventArgs e) {
        StartCoroutine(AttackCoroutine());
    }

    public void OnRoundEnd(System.Object src, EventArgs e) {
        StopAllCoroutines();
        Cleanup();
    }

    public void LoseGem() {
        //we die when we lose the gem
        Die();
    }


    private IEnumerator DeathCoroutine() {
        //we will simultaneously point the turret upward and sink into the ground
        float y0 = transform.position.y;
        float yf = -0.5f;
        Quaternion q0 = turret.transform.GetChild(0).localRotation;
        Quaternion qf = Quaternion.Euler(0, 0, -90);

        //todo: play a 1s whirring sound

        float duration = 1.0f;

        while (duration > 0.0f) {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(y0, yf, 1 - duration), transform.position.z);
            turret.transform.GetChild(0).localRotation = Quaternion.Lerp(q0, qf, 1 - duration);
            duration -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.position = new Vector3(transform.position.x, yf, transform.position.z);
        turret.transform.GetChild(0).localRotation = qf;

        yield return null;
    }


    private IEnumerator AttackCoroutine() {
        //todo: pick a random attack pattern (or cycle through them)
        float attackDist = 10; //distance in m
        float rotationTime = 1.0f;
        float attackTime = 3.0f;

        //we call StopAllCoroutines at the end of the round
        while (BattleTimeManager.instance.PlayerActing()) {
            //pick a straight line + look at the player & random offset
            Vector3 lookTarget = playerTransform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), 0, UnityEngine.Random.Range(-2f, 2f)) - turret.transform.position;
            lookTarget.y = 0;
            StartCoroutine(LookCoroutine(lookTarget, rotationTime));

            //highlight the hexagons in a line -- get this from the attack pattern
            float x0 = turret.position.x;
            float z0 = turret.position.z;
            Quaternion qf = Quaternion.LookRotation(lookTarget, Vector3.up);

            //first we will get a vector of attack locations, the HashSet ensures they're unique (10/10 lazyness)
            attackLocations.Clear();
            for (float d = 0; d <= attackDist; d += 0.5f) {
                Vector3 disp = qf * (new Vector3(0, 0, d));
                Vector2Int idx = GetTileIndexFromPosition(x0 + disp.x, z0 + disp.z);

                attackLocations.Add(idx);
            }
            //then we spaw a tileoutline (+ hit ParticleEffect?) at each location
            foreach (Vector2Int pos in attackLocations) {
                GameObject go = Instantiate(tileOutline);
                SetPosition(go, pos.x, 0, pos.y);
                tileOutlines.Add(go);
            }

            //wait to rotate
            yield return new WaitForSeconds(rotationTime);


            //***** attack now!
            audioSrc.Play(); // this lasts 3 seconds
            //play some particle effect + sfx + add smoke
            foreach (var ps in fireParticles) {
                ps.Play();
            }
            //play some "on tile hit" particle effect

            //damage the player if they enter any of the tiles we are attacking
            
            //wait for the attack to finish
            yield return new WaitForSeconds(attackTime);

            Cleanup();

            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    void Cleanup() {
        //***** clean up the attack
        // stop the particles and attack points
        foreach (var ps in fireParticles) {
            ps.Stop();
        }
        foreach (var ps in smokeParticles) {
            ps.Play();
        }
        //clean up tile outlines
        for (int i = tileOutlines.Count - 1; i >= 0; i--) {
            Destroy(tileOutlines[i]);
        }
        tileOutlines.Clear();
        audioSrc.Stop();
    }


    public IEnumerator LookCoroutine(Vector3 lookDir, float duration) {
        Quaternion q0 = turret.rotation;
        Quaternion qf = Quaternion.LookRotation(lookDir, Vector3.up);
        float elapsed = 0;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            turret.rotation = Quaternion.Lerp(q0, qf, elapsed / duration);
            yield return new WaitForEndOfFrame();
        }

        turret.rotation = qf;

        yield return null;
    }


}
