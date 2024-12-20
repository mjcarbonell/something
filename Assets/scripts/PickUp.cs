using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (heldObj == null)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                    {
                        if (hit.transform.gameObject.CompareTag("canPickUp"))
                        {
                            CmdPickUpObject(hit.transform.gameObject); // Command to pick up the object
                        }
                    }
                }
                else
                {
                    if (canDrop)
                    {
                        CmdDropObject(); // Command to drop the object
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (heldObj != null)
                {
                    CmdThrowObject(); // Command to throw the object
                }
            }
        }
    }

    [Command]
    void CmdPickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;

            heldObj.transform.SetParent(holdPos.transform); // Set parent on the server
            heldObj.transform.position = holdPos.position;
            heldObj.transform.rotation = holdPos.rotation;

            heldObj.layer = LayerNumber;

            // Sync with all clients
            RpcPickUpObject(heldObj);
        }
    }

    [ClientRpc]
    void RpcPickUpObject(GameObject pickUpObj)
    {
        if (!isServer)
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;

            heldObj.transform.SetParent(holdPos.transform);
            heldObj.transform.position = holdPos.position;
            heldObj.transform.rotation = holdPos.rotation;

            heldObj.layer = LayerNumber;
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
