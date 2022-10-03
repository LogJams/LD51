using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitfallManager : MonoBehaviour
{
    public GameObject PitfallCube;
    public GameObject PitfallLine;

    private GameObject instancePitfallCube1;
    private GameObject instancePitfallCube2;
    private GameObject instancePitfallLine;
    private LineRenderer pitfallLine;

    private Vector3 pitFallHeight = new Vector3(0, 1.125f, 0);

    private GameObject player;
    private PlayerManager playerManager;

    private Vector3 height = Vector3.up;

    bool unlocked = false;

    public event System.EventHandler OnPitfallStateChange;

    public enum PitfallStates
    {
        down = 0,
        settingUp = 1,
        setUp = 2
    }

    private PitfallStates _pitfallState = PitfallStates.down;
    public PitfallStates pitfallState { get { return _pitfallState; } private  set { _pitfallState = value; OnPitfallStateChange?.Invoke(this, System.EventArgs.Empty); } }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerManager = player.GetComponent<PlayerManager>();

        playerManager.OnUnlockPitfall += UnlockPitfall;

    }

    public void UnlockPitfall(System.Object src, System.EventArgs e) {
        unlocked = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!unlocked) return;

        
        if (Input.GetKeyUp("p"))
        {
            if (pitfallState == PitfallStates.down)
            {
                // Instantiate cubes
                instancePitfallCube1 = Instantiate(PitfallCube, playerManager.currentTilePositon + pitFallHeight, Quaternion.identity);
                instancePitfallCube2 = Instantiate(PitfallCube, playerManager.currentTilePositon + pitFallHeight, Quaternion.identity);
                
                // Instantiate line
                instancePitfallLine= Instantiate(PitfallLine);
                pitfallLine = instancePitfallLine.GetComponent<LineRenderer>();

                // Fix line position of first cube
                pitfallLine.SetPosition(0, instancePitfallCube1.transform.position);

                // Update status of pitfall
                pitfallState = PitfallStates.settingUp;
            }
            else if (pitfallState == PitfallStates.settingUp)
            {
                pitfallState = PitfallStates.setUp;
                PitfallLineCollision colliderScript = instancePitfallLine.GetComponent<PitfallLineCollision>();
                colliderScript.Setup();
                colliderScript.parent = this;

            } else { 
                ResetPitfall();
            }
        } else
        {
            if (pitfallState == PitfallStates.settingUp)
            {
                // Render the line
                instancePitfallCube2.transform.position = playerManager.currentTilePositon + pitFallHeight;
                pitfallLine.SetPosition(1, instancePitfallCube2.transform.position);

                // Let cubes face each other
                instancePitfallCube1.transform.LookAt(instancePitfallCube2.transform);
                instancePitfallCube2.transform.LookAt(instancePitfallCube1.transform);
            }
        }
    }

    private void ResetPitfall()
    {
        Destroy(instancePitfallCube1);
        Destroy(instancePitfallCube2);
        Destroy(instancePitfallLine);
        pitfallState = PitfallStates.down;
    }


    public void RecieveTrigger(GameObject source) {
        //see if we use the pitfall trap on an enemy
        Enemy e = source.GetComponent<Enemy>();
        if (e != null && e.OnPitfallTrap()) {
            ResetPitfall();
        }

        //if this isn't an enemy (e.g., it's a wall) just reset
        if (e == null) {
            ResetPitfall();
        }
    }
}
