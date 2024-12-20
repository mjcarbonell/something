using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickUp : NetworkBehaviour
{
    public Camera playerCamera;
    public Transform holdPos;
    public float throwForce = 500f;
    public float pickUpRange = 5f;

    private GameObject heldObj;
    private Rigidbody heldObjRb;

    private int holdLayer;

    void Start()
    {
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(isLocalPlayer); // Enable the camera only for the local player
        }
        holdLayer = LayerMask.NameToLayer("holdLayer");
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (heldObj == null)
                {
                    TryPickUpObject();
                }
                else
                {
                    DropObject();
                }
            }

            if (Input.GetKeyDown(KeyCode.F) && heldObj != null)
            {
                ThrowObject();
            }

            if (heldObj != null)
            {
                MoveHeldObject();
            }
        }
    }

    void TryPickUpObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, pickUpRange))
        {
            if (hit.collider.CompareTag("canPickUp"))
            {
                CmdPickUpObject(hit.collider.gameObject);
            }
        }
    }

    [Command]
    void CmdPickUpObject(GameObject obj)
    {
        RpcPickUpObject(obj, holdPos.gameObject);
    }

    [ClientRpc]
    void RpcPickUpObject(GameObject obj, GameObject holder)
    {
        heldObj = obj;
        heldObjRb = obj.GetComponent<Rigidbody>();

        if (heldObjRb != null)
        {
            heldObjRb.isKinematic = true; // Disable physics
            heldObj.transform.SetParent(holder.transform); // Parent to the holder
            heldObj.transform.localPosition = Vector3.zero; // Reset local position
            heldObj.layer = holdLayer; // Set to "holdLayer" to avoid collisions
        }
    }

    void DropObject()
    {
        if (heldObj != null)
        {
            CmdDropObject();
        }
    }

    [Command]
    void CmdDropObject()
    {
        RpcDropObject();
    }

    [ClientRpc]
    void RpcDropObject()
    {
        if (heldObj != null)
        {
            heldObj.layer = 0; // Reset to default layer
            heldObjRb.isKinematic = false; // Re-enable physics
            heldObj.transform.SetParent(null); // Unparent the object
            heldObj = null;
        }
    }

    void ThrowObject()
    {
        if (heldObj != null)
        {
            CmdThrowObject(playerCamera.transform.forward);
        }
    }

    [Command]
    void CmdThrowObject(Vector3 direction)
    {
        RpcThrowObject(direction);
    }

    [ClientRpc]
    void RpcThrowObject(Vector3 direction)
    {
        if (heldObj != null)
        {
            heldObj.layer = 0; // Reset to default layer
            heldObjRb.isKinematic = false; // Re-enable physics
            heldObj.transform.SetParent(null); // Unparent the object
            heldObjRb.AddForce(direction * throwForce); // Add force for throwing
            heldObj = null;
        }
    }

    void MoveHeldObject()
    {
        if (heldObj != null)
        {
            heldObj.transform.position = holdPos.position; // Keep the object at the hold position
            heldObj.transform.rotation = holdPos.rotation;
        }
    }
}
