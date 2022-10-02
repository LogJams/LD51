using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera _mainCamera;

    private Vector3 _viewOffset = new Vector3(0.0f, 15.0f, 0.0f);

    private Vector3 _oldPlayerPosition;

    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
        _mainCamera.transform.position = transform.position + _viewOffset;
        _oldPlayerPosition = transform.position;
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
}
