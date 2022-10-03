using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using static TerrainHelpers;
using static HexagonHelpers;

public class PlayerManager : MonoBehaviour {

    public event EventHandler OnUnlockPitfall;
    public event EventHandler OnHealthChange; //the UI can use this to update the health
    public event EventHandler OnDeath;

    public GameObject player;

    public bool isMoving = false;
    private float movingSpeed = 3.0f;

    private CharacterController controller;
    Vector3 movement;
    float verticalSpeed = 0;

    public Vector3 currentTilePositon = new Vector3();


    public float interactionRange = 2.5f;

    //this is a fancy C# thing, anything can read health but only PlayerManager can write to it
    private int _health;
    public int Health { get { return _health;}  
                        private set { _health = value; 
                                      OnHealthChange?.Invoke(this, EventArgs.Empty);
                                      if (_health <= 0) Die();
                                     } }
    [SerializeField] int MaxHealth = 5;
    [SerializeField] float InvulnerabilityTime = 1.0f; //how long should we be invulnerable?
    float hpTimer;


    public void Awake()
    {
        Health = MaxHealth;
        controller = player.GetComponent<CharacterController>();
    }


    public void UnlockPitfall() {
        OnUnlockPitfall?.Invoke(this, EventArgs.Empty);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.GetComponent<Enemy>() != null && hpTimer <= 0) {
            
            hpTimer = InvulnerabilityTime;

            //should we do double damage if it's the boss?
            HuntingBoss hb = collision.gameObject.GetComponent<HuntingBoss>();
            if (hb != null && hb.isAlive) {
                Health-= 2;
            }
            else {
                Health--;
            }

        }
    }

    public void TakeDamage(int qty) {
        Health -= qty;
    }


    void Die() {
        //todo: switch to the game over scene after a fade out
        //perhaps play a sad "wah-wah" trumpet sound (just kidding {?} )

        //tell everyone we died - UI Main will fade out and end the game
        OnDeath?.Invoke(this, EventArgs.Empty);
    }


    public void SetCurrentTilePosition(Vector3 currentTile)
    {
        currentTilePositon = currentTile;
    }

    public void Update() {
        if (Health <= 0) return;


        verticalSpeed += Physics.gravity.y * Time.deltaTime;
        if (controller.isGrounded) {
            verticalSpeed = 0;
        }
        if (!isMoving) {
            movement = Vector3.zero;
        }
        controller.Move( (movement + new Vector3(0, verticalSpeed, 0) ) * Time.deltaTime);

        hpTimer -= Time.deltaTime; //track how long it's been since we last took damage

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo)) {
                Vector3 dp = hitInfo.collider.gameObject.transform.position - transform.position;
                dp.y = 0;
                if (dp.sqrMagnitude <= interactionRange*interactionRange) {
                    Interactable intbl = hitInfo.collider.gameObject.GetComponent<Interactable>();
                    if (intbl != null) {
                        intbl.Interact(this);
                    }
                }

            }


        }

    }


    public void MovePlayer(List<Vector2Int> currentpath, List<GameObject> pathIndicators)
    {
        if (!isMoving) {
            return;
        }

        if (currentpath.Count > 0)
        {
            Vector2Int goalPosition = currentpath.Last();
            Vector3 tilePosition = GetPositionFromIndex(goalPosition);
            Vector3 projectedPlayerPosition = new Vector3(player.transform.position.x, 0, player.transform.position.z);

            if ((tilePosition - projectedPlayerPosition).magnitude < 0.1)
            {
                //tell the game we arrived at a new tile
                OverworldManager.instance.PlayerMovement(goalPosition);

                // Keep track of the current tile position and the previous one
                currentTilePositon = tilePosition;

                // Remove the way point
                currentpath.RemoveAt(currentpath.Count - 1);

                // Remove the path indicator that was reached
                if (pathIndicators.Count > 0)
                {
                    Destroy(pathIndicators[pathIndicators.Count - 1]);
                    pathIndicators.RemoveAt(pathIndicators.Count - 1);
                }

                // If this was the last way-point we are done moving
                if (currentpath.Count == 0)
                    isMoving = false;
            }
            else
            {
                movement = (tilePosition - projectedPlayerPosition).normalized * movingSpeed;
            }
        } else
        {
            isMoving = false;
        }
    }
}
