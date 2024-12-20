using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine;

public class BallSync : NetworkBehaviour
{
    private Rigidbody rb;

    [SyncVar] private Vector3 syncPosition;
    [SyncVar] private Quaternion syncRotation;
    [SyncVar] private Vector3 syncVelocity;
    [SyncVar] private Vector3 syncAngularVelocity;
    private void Start(){
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate(){
        if (hasAuthority) // Only the authoritative player updates the state
        {
            CmdSyncState(transform.position, transform.rotation, rb.velocity, rb.angularVelocity);
        }
        else{
            // Apply the synced state for non-authoritative players
            rb.position = syncPosition;
            rb.rotation = syncRotation;
            rb.velocity = syncVelocity;
            rb.angularVelocity = syncAngularVelocity;
        }
    }
    [Command]
    private void CmdSyncState(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity){
        // Update the SyncVars
        syncPosition = position;
        syncRotation = rotation;
        syncVelocity = velocity;
        syncAngularVelocity = angularVelocity;
    }
}
