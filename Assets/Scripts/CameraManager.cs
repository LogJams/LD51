using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private GameObject player;

    // Fixed camera boom
    private static Vector3 cameraBoom = new Vector3(-23.9f, 61.51f, -25.024f);
    private static Vector3 projectedCameraBoom = new Vector3(-23.9f, 0.0f, -25.024f);

    private Camera _mainCamera;

    bool initialized = false;

    // Start is called before the first frame update
    void Awake()
    {
        // Find the camera
        _mainCamera = Camera.main;

        // Find the player
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Start()
    {
        OverworldManager.instance.OnPlayerEnterRoom += OnEnterRoom;
        OverworldManager.instance.OnPlayerExitRoom += OnLeaveRoom;
    }

    public void OnEnterRoom(System.Object src, System.EventArgs e)
    {
        Zone room = (Zone)src;

        // Find the position of the middle(ish) hexagon
        Vector2Int roomMiddle = new Vector2Int(room.x + (int)(0.5f * room.w), room.y + (int)(0.5f * room.h));
        Vector3 roomPos = HexagonHelpers.GetPositionFromIndex(roomMiddle);

        // Run the coroutine
        if (initialized)
            StartCoroutine(PanToPositionCoroutine(roomPos));
        else
        {
            _mainCamera.transform.position = roomPos + cameraBoom;
            initialized = true;
        }
    }

    public void OnLeaveRoom(System.Object src, System.EventArgs e)
    {
        StartCoroutine(PanToPositionCoroutine(player.transform.position));
    }

    private IEnumerator PanToPositionCoroutine(Vector3 pos)
    {
        float elapsed = 0;
        float cameraPanSpeed = 10.0f;

        Vector3 projectedPos = new Vector3(pos.x, 0, pos.z);
        Vector3 projectedCameraPos = new Vector3(_mainCamera.transform.position.x, 0, _mainCamera.transform.position.z);

        float duration = (projectedPos - projectedCameraPos + projectedCameraBoom).magnitude / cameraPanSpeed;
        Vector3 panDirection = (projectedPos - projectedCameraPos + projectedCameraBoom).normalized;

        // Ignore the y coordinate
        panDirection.y = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _mainCamera.transform.position += cameraPanSpeed * panDirection * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        _mainCamera.transform.position = projectedPos + cameraBoom;

        yield return null;
    }
}
