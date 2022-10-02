using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static HexagonHelpers;

public class TurretBoss : MonoBehaviour {

    public Transform turret;
    public GameObject tileOutline;

    Transform playerTransform;


    bool actionPhase = false; //when true, the turret should be attacking

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

        actionPhase = true;
        StartCoroutine(AttackCoroutine());

    }

    // Update is called once per frame
    void Update() {

    }

    

    public IEnumerator AttackCoroutine() {
        //todo: pick a random attack pattern (or cycle through them)
        float attackDist = 10; //distance in m
        float rotationTime = 1.0f;
        float attackTime = 3.0f;

        while (actionPhase) {
            //pick a straight line + look at the player & random offset
            Vector3 lookTarget = playerTransform.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f)) - turret.transform.position;
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
            //play some particle effect + sfx + add smoke
            foreach (var ps in fireParticles) {
                ps.Play();
            }
            //play some "on tile hit" particle effect

            //damage the player if they enter any of the tiles we are attacking

            //wait for the attack to finish
            yield return new WaitForSeconds(attackTime);

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
