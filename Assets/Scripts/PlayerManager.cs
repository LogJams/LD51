using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using static TerrainHelpers;
using static HexagonHelpers;

public class PlayerManager : MonoBehaviour
{
    public GameObject player;

    public bool isMoving = false;
    private float movingSpeed = 3.0f;

    private CharacterController controller;
    Vector3 movement;
    float verticalSpeed = 0;

    public Vector3 currentTilePositon = new Vector3();

    public void Awake()
    {
        controller = player.GetComponent<CharacterController>();
    }

    public void SetCurrentTilePosition(Vector3 currentTile)
    {
        currentTilePositon = currentTile;
    }

    public void Update() {
        verticalSpeed += Physics.gravity.y * Time.deltaTime;
        if (controller.isGrounded) {
            verticalSpeed = 0;
        }
        if (!isMoving) {
            movement = Vector3.zero;
        }
        controller.Move( (movement + new Vector3(0, verticalSpeed, 0) ) * Time.deltaTime);
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
