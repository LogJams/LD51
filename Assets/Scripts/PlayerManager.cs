using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using static TerrainHelpers;
using static HexagonHelpers;

public class PlayerManager : MonoBehaviour
{
    public bool isMoving = false;
    private float movingSpeed = 3.0f;

    private GameObject player;
    private CharacterController controller;

    public PlayerManager(GameObject _player)
    {
        player = _player;
        controller = player.GetComponent<CharacterController>();
    }

    public void MovePlayer(List<Vector2Int> currentpath)
    {
        if (!isMoving)
            return;

        if (currentpath.Count > 0)
        {
            Vector2Int goalPosition = currentpath.Last();
            Vector3 tilePosition = GetPositionFromIndex(goalPosition);
            Vector3 projectedPlayerPosition = new Vector3(player.transform.position.x, 0, player.transform.position.z);

            if ((tilePosition - projectedPlayerPosition).magnitude < 0.1)
            {
                currentpath.RemoveAt(currentpath.Count - 1);

                if (currentpath.Count == 0)
                    isMoving = false;
            }
            else
            {
                controller.Move((tilePosition - projectedPlayerPosition).normalized * Time.deltaTime * movingSpeed);
            }
        }
        else
        {
            // Shouldn't enter this, but let's make sure 
            throw new Exception("Why are we entering this code?");
        }
    }
}
