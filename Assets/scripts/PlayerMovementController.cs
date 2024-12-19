using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror; 
using UnityEngine.SceneManagement; 

public class PlayerMovementController : NetworkBehaviour
{
    public float speed = 1f;
    public GameObject PlayerModel; 
    private Camera playerCamera;
    private void Start(){
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(isLocalPlayer); // Enable camera only for the local player
        }
        PlayerModel.SetActive(false); 
    }
    // working cameras
    private void Update(){
        if(SceneManager.GetActiveScene().name == "Scene_SteamworksGame"){
            if(PlayerModel.activeSelf == false){
                SetPosition(); 
                PlayerModel.SetActive(true); 
            }
            if(hasAuthority){
                Movement(); 
            }
        }
    }
    public void Movement (){
        float xDirection = Input.GetAxis("Horizontal"); 
        float zDirection = Input.GetAxis("Vertical"); 
        Vector3 moveDirection = new Vector3(xDirection, 0.0f,zDirection);
        transform.position += moveDirection * speed; 
    }
    public void SetPosition(){
        transform.position = new Vector3(Random.Range(-5,5), 0.8f, Random.Range(-5,5)); 
    }
}
