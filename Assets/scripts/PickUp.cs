using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickUp : NetworkBehaviour
{
    public Camera playerCamera;
    public GameObject player;
    public Transform holdPos; // Position where the held object will be attached
    public float throwForce = 500f;
    public float pickUpRange = 5f;
    private GameObject heldObj;
    private Rigidbody heldObjRb;
    private int LayerNumber;

    void Start()
    {
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(isLocalPlayer); // Enable camera only for the local player
        }
        LayerNumber = LayerMask.NameToLayer("holdLayer");
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (playerCamera != null)
            {
                playerCamera.gameObject.SetActive(true);
            }
            if (holdPos != null)
            {
                holdPos.gameObject.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.E)){
                if (heldObj == null)
                {
                    // Try to pick up an object
                    RaycastHit hit;
                    if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                    {
                        if (hit.transform.gameObject.CompareTag("canPickUp"))
                        {
                            CmdPickUpObject(hit.transform.gameObject, netIdentity); // Pass the object and player's NetworkIdentity
                        }
                    }
                }
                else
                {
                    // Drop the object
                    CmdDropObject();
                }
            }

            if (Input.GetKeyDown(KeyCode.F) && heldObj != null)
            {
                // Throw the object
                CmdThrowObject();
            }
        }
    }

    [Command]
    void CmdPickUpObject(GameObject pickUpObj, NetworkIdentity playerIdentity)
    {
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;

            // Assign the object's parent to the player's holdPos
            Transform holdPosition = playerIdentity.GetComponent<PickUp>().holdPos;
            pickUpObj.transform.SetParent(holdPosition);

            // Sync with all clients
            RpcPickUpObject(pickUpObj, playerIdentity);
        }
    }

    [ClientRpc]
    void RpcPickUpObject(GameObject pickUpObj, NetworkIdentity playerIdentity)
    {
        if (pickUpObj != null)
        {
            Transform holdPosition = playerIdentity.GetComponent<PickUp>().holdPos;

            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;

            pickUpObj.transform.SetParent(holdPosition);
            pickUpObj.transform.position = holdPosition.position;
            pickUpObj.transform.rotation = holdPosition.rotation;
        }
    }

    [Command]
    void CmdDropObject()
    {
        if (heldObj != null)
        {
            RpcDropObject();
        }
    }

    [ClientRpc]
    void RpcDropObject()
    {
        if (heldObj != null)
        {
            heldObj.layer = 0;
            heldObjRb.isKinematic = false;
            heldObj.transform.SetParent(null);
            heldObj = null;
        }
    }

    [Command]
    void CmdThrowObject()
    {
        if (heldObj != null)
        {
            RpcThrowObject(playerCamera.transform.forward);
        }
    }

    [ClientRpc]
    void RpcThrowObject(Vector3 direction)
    {
        if (heldObj != null)
        {
            heldObj.layer = 0;
            heldObjRb.isKinematic = false;
            heldObj.transform.SetParent(null);
            heldObjRb.AddForce(direction * throwForce);
            heldObj = null;
        }
    }
}
