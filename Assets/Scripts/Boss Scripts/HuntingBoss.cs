using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

using static HexagonHelpers;

public class HuntingBoss : MonoBehaviour, Enemy {

    public Transform hunter;
    public GameObject tileOutline;

    Transform playerTransform;

    public List<ParticleSystem> fireParticles;
    public List<ParticleSystem> smokeParticles;

    HashSet<Vector2Int> attackLocations;
    List<GameObject> tileOutlines;

    private void Awake() {
        tileOutlines = new List<GameObject>();
        attackLocations = new HashSet<Vector2Int>();
    }
    // Start is called before the first frame update
    void Start() {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        BattleTimeManager.instance.OnTimerStart += OnRoundStart;
        BattleTimeManager.instance.OnTimerEnd += OnRoundEnd;
    }

    public void OnRoundStart(System.Object src, EventArgs e) {
        StartCoroutine(AttackCoroutine());
    }

    public void OnRoundEnd(System.Object src, EventArgs e) {
        StopAllCoroutines();
    }

    public bool OnPitfallTrap()
    {
        StartCoroutine(FallCoroutine());
        return true;
    }

    public IEnumerator AttackCoroutine() {
        //todo: pick a random attack pattern (or cycle through them)
        float rotationTime = 1.0f;
        float moveSpeed = 5.0f;
        float waitTime = 1.0f;

        //we call StopAllCoroutines at the end of the round
        while (BattleTimeManager.instance.PlayerActing()) {
            //pick a straight line + look at the player & random offset
            Vector3 lookTarget = playerTransform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), 0, UnityEngine.Random.Range(-2f, 2f)) - hunter.transform.position;
            Vector3 runTarget = playerTransform.position;

            lookTarget.y = 0;
            StartCoroutine(LookCoroutine(lookTarget, rotationTime));

            //highlight the hexagons in a line -- get this from the attack pattern
            float x0 = hunter.position.x;
            float z0 = hunter.position.z;
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
            foreach (var ps in fireParticles) {
                ps.Stop();
            }
            foreach (var ps in smokeParticles) {
                ps.Play();
            }
            //clean up tile outlines
            for (int i = tileOutlines.Count-1; i >= 0; i--) {
                Destroy(tileOutlines[i]);
            }
            tileOutlines.Clear();

            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    public IEnumerator LookCoroutine(Vector3 lookDir, float duration) {
        Quaternion q0 = hunter.rotation;
        Quaternion qf = Quaternion.LookRotation(lookDir, Vector3.up);
        float elapsed = 0;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            hunter.rotation = Quaternion.Lerp(q0, qf, elapsed / duration);
            yield return new WaitForEndOfFrame();
        }

        hunter.rotation = qf;

        yield return null;
    }

    public IEnumerator FallCoroutine()
    {
        float fallDirection = 0.5f;
        Quaternion q0 = hunter.rotation;
        Quaternion qf = Quaternion.LookRotation(-Vector3.up, hunter.transform.forward);
        float elapsed = 0;

        while (elapsed < fallDirection)
        {
            elapsed += Time.deltaTime;
            hunter.rotation = Quaternion.Lerp(q0, qf, elapsed / fallDirection);
            yield return new WaitForEndOfFrame();
        }

        hunter.rotation = qf;

        yield return null;
    }

    public IEnumerator RunCoroutine(Vector3 runTarget, float speed)
    {
        float elapsed = 0;
        float duration = (runTarget - hunter.transform.position).magnitude/speed;
        Vector3 runDirection = (runTarget - hunter.transform.position).normalized;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            hunter.transform.position += speed * runDirection * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        hunter.transform.position = runTarget;

        yield return null;
    }

}
