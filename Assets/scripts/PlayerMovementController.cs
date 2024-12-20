using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovementController : NetworkBehaviour
{
    [SyncVar] public float speed;
    public GameObject PlayerModel;
    public GameObject holdPos;
    private Camera playerCamera;
    public float mouseSensitivity = 100f; // Sensitivity for mouse movement
    public Transform playerBody;          // Reference to the player's body (root object)
    private float xRotation = 0f;         // Track the camera's up/down rotation

    private void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(isLocalPlayer); // Enable camera only for the local player
        }
        PlayerModel.SetActive(false);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Scene_SteamworksGame")
        {
            if (PlayerModel.activeSelf == false)
            {
                SetPosition();
                Cursor.lockState = CursorLockMode.Locked;
                PlayerModel.SetActive(true);
            }
            if (hasAuthority)
            {
                Movement();
                CameraMovement();
            }

            // Rotate holdPos for local players
            if (isLocalPlayer)
            {
                UpdateHoldPosRotation();
            }
        }
    }

    public void Movement()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");

        if (xDirection != 0 || zDirection != 0)
        {
            Vector3 moveDirection = playerBody.transform.right * xDirection + playerBody.transform.forward * zDirection;
            moveDirection = moveDirection.normalized;
            transform.position += moveDirection * speed * Time.deltaTime;
        }
    }

    public void CameraMovement()
    {
        if (!isLocalPlayer) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        playerBody.Rotate(Vector3.up * mouseX);
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Quaternion cameraRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerCamera.transform.localRotation = cameraRotation;
    }

    private void UpdateHoldPosRotation()
    {
        // Ensure holdPos follows the local player's camera rotation
        if (playerCamera != null && holdPos != null)
        {
            holdPos.transform.rotation = playerCamera.transform.rotation;
        }
    }

    public void SetPosition()
    {
        transform.position = new Vector3(Random.Range(-5, 5), 0.8f, Random.Range(-5, 5));
    }
}
