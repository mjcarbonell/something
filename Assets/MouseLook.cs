using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror; 
using UnityEngine.SceneManagement; 
public class MouseLook : NetworkBehaviour
{
    public float mouseSensitivity = 100f; // Sensitivity for mouse movement
    public Transform playerBody;          // Reference to the player's body (root object)
    private float xRotation = 0f;         // Track the camera's up/down rotation

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor in the center of the screen
    }

    void Update()
    {
        if (!isLocalPlayer) return; // Ensure only the local player can control the camera

        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the player body along the Y-axis (left/right rotation)
        playerBody.Rotate(Vector3.up * mouseX);

        // Rotate the camera along the X-axis (up/down rotation)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamping to avoid looking too far up/down
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Apply rotation to the camera
    }
}
