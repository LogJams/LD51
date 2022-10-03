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

    private Vector3 currentTarget;

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
        // Look at player
        _mainCamera.transform.position = player.transform.position + cameraBoom;
        currentTarget = player.transform.position;
    }

    void Update()
    {
        float bias = 3.0f;

        // Lerp after the player
        currentTarget = player.transform.position + cameraBoom;
        _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, currentTarget, bias * Time.deltaTime); ;
    }
}
