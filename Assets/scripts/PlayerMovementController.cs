using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror; 
using UnityEngine.SceneManagement; 
public class PlayerMovementController : NetworkBehaviour
{
    [SyncVar] public float speed = 1f;
    public GameObject PlayerModel;
    private Camera playerCamera;

    private void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(isLocalPlayer); // Enable camera only for local player
        }

        if (SceneManager.GetActiveScene().name == "Scene_SteamworksGame")
        {
            SetPosition(); // Randomly set initial position
        }

        PlayerModel.SetActive(isLocalPlayer ? false : true); // Hide for local player
    }

    private void Update()
    {
        if (!isLocalPlayer || SceneManager.GetActiveScene().name != "Scene_SteamworksGame")
        {
            return;
        }

        Movement();
    }

    public void Movement()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(xDirection, 0.0f, zDirection).normalized;

        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    public void SetPosition()
    {
        transform.position = new Vector3(Random.Range(-5, 5), 0.8f, Random.Range(-5, 5));
    }
}