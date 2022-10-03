using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private GameObject player;

    // Fixed camera boom
    private static Vector3 cameraBoom = new Vector3(-35.5f, 52.5f, -35.1f);

    private Camera _mainCamera;

    private Vector3 _viewOffset = new Vector3(0.0f, 15.0f, 0.0f);

    private Vector3 _oldPlayerPosition;

    // Start is called before the first frame update
    void Awake()
    {
        _mainCamera = Camera.main;
        _mainCamera.transform.position = transform.position + _viewOffset;
        _oldPlayerPosition = transform.position;

        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Start()
    {
        OverworldManager.instance.OnPlayerEnterRoom += OnEnterRoom;
        OverworldManager.instance.OnPlayerExitRoom += OnLeaveRoom;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the camera
        UpdateCamera();

        // Keep track of old player position
        _oldPlayerPosition = transform.position;
    }

    /// <summary>
    /// Update the camera position according to player motion
    /// </summary>
    void UpdateCamera()
    {
        _mainCamera.transform.position += transform.position - _oldPlayerPosition;
    }

    public void OnEnterRoom(System.Object src, System.EventArgs e)
    {
        Zone room = (Zone)src;

        // Find the position of the middle(ish) hexagon
        Vector2Int roomMiddle = new Vector2Int(room.x + (int)(0.5f * room.w), room.y + (int)(0.5f * room.h));
        Vector3 roomPos = HexagonHelpers.GetPositionFromIndex(roomMiddle);

        // Run the coroutine
        StartCoroutine(PanToPositionCoroutine(roomPos));
    }

    public void OnLeaveRoom(System.Object src, System.EventArgs e)
    {
        StartCoroutine(PanToPositionCoroutine(player.transform.position));
    }

    private IEnumerator PanToPositionCoroutine(Vector3 pos)
    {
        float elapsed = 0;
        float cameraPanSpeed = 2.0f;

        float duration = (pos - transform.position + cameraBoom).magnitude / cameraPanSpeed;
        Vector3 panDirection = (pos - transform.position + cameraBoom).normalized;

        // Ignore the y coordinate
        panDirection.y = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position += cameraPanSpeed * panDirection * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.position = pos + cameraBoom;

        yield return null;
    }
}
