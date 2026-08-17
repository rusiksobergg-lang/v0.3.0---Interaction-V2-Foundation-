
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerZoom : MonoBehaviour
{
    public Camera playerCamera;

    public float normalFOV = 75f;
    public float zoomFOV = 45f;
    public float zoomSpeed = 8f;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        playerCamera.fieldOfView = normalFOV;
    }

    void Update()
    {
        float targetFOV = Mouse.current.middleButton.isPressed ? zoomFOV : normalFOV;

        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            targetFOV,
            zoomSpeed * Time.deltaTime
        );
    }
}