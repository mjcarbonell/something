using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror; 
using UnityEngine.SceneManagement; 
public class PickUp : NetworkBehaviour
{
    public Camera playerCamera;
    public GameObject player;
    public Transform holdPos;
    public float throwForce = 500f;
    public float pickUpRange = 5f;
    private float rotationalSensitivity = 1f;
    private GameObject heldObj;
    private Rigidbody heldObjRb;
    private bool canDrop = true;
    private int LayerNumber;

    void Start()
    {
        if (playerCamera != null){
            playerCamera.gameObject.SetActive(isLocalPlayer); // Enable camera only for the local player
        }
        LayerNumber = LayerMask.NameToLayer("holdLayer");
    }
    void Update(){
        if(isLocalPlayer){
            if(Input.GetKeyDown(KeyCode.G)){
                if(heldObj != null){
                }
            }
            if(Input.GetKeyDown(KeyCode.E)){
                if(heldObj == null){
                    RaycastHit hit;
                    if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.TransformDirection(Vector3.forward), out hit, pickUpRange)){
                        Debug.Log(hit.transform.gameObject.tag);
                        if(hit.transform.gameObject.tag == "canPickUp"){
                            pickUpObject(hit.transform.gameObject);       
                        }
                    }
                }
                else{
                    if(canDrop == true){
                        StopClipping();
                        dropObject();
                    }
                } 
            }
            if(Input.GetKeyDown(KeyCode.F)){
                if(heldObj != null){
                    StopClipping();
                    ThrowObject();
                }
            }
        }
        
    }

    void pickUpObject(GameObject pickUpObj){
        if(pickUpObj.GetComponent<Rigidbody>()){
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform;

            // Set object position and rotation relative to the holding position
            heldObj.transform.position = holdPos.position;
            heldObj.transform.rotation = holdPos.rotation;

            heldObj.layer = LayerNumber;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
    }

    void dropObject(){
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObjRb.transform.parent = null;
        heldObj = null;
    }

    void MoveObject(){
        if (heldObj != null) {
            // Ensure the object maintains the same position and rotation
            heldObj.transform.position = holdPos.position;
            heldObj.transform.rotation = holdPos.rotation;
        }
    }

    void ThrowObject(){
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObjRb.transform.parent = null;
        heldObjRb.AddForce(playerCamera.transform.forward*throwForce);
        heldObj = null;
    }
    
    void StopClipping(){
        var clipRange = Vector3.Distance(heldObj.transform.position, playerCamera.transform.position);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(playerCamera.transform.position, playerCamera.transform.TransformDirection(Vector3.forward), clipRange);
        if(hits.Length > 1){
            heldObj.transform.position = playerCamera.transform.position + new Vector3(0f, -0.5f, 0f);
        }
    }
}
