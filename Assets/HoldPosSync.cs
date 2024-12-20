using Mirror;
using UnityEngine;

public class HoldPosSync : NetworkBehaviour
{
    [SyncVar] private Vector3 syncPosition; // SyncVar for position
    [SyncVar] private Quaternion syncRotation; // SyncVar for rotation
    private Transform holdPosTransform;
    private void Start()
    {
        holdPosTransform = transform;
    }
    private void FixedUpdate()
    {
        if (isServer) // Server updates the SyncVars
        {
            syncPosition = holdPosTransform.position;
            syncRotation = holdPosTransform.rotation;
        }
        else // Non-server clients interpolate to the server state
        {
            holdPosTransform.position = Vector3.Lerp(holdPosTransform.position, syncPosition, Time.deltaTime * 10);
            holdPosTransform.rotation = Quaternion.Lerp(holdPosTransform.rotation, syncRotation, Time.deltaTime * 10);
        }
    }

    // Command to move and rotate the holdPos from the client
    [Command(requiresAuthority = false)]
    public void CmdMoveHoldPos(Vector3 newPosition, Quaternion newRotation)
    {
        // Update the holdPos position and rotation on the server
        holdPosTransform.position = newPosition;
        holdPosTransform.rotation = newRotation;

        // SyncVars are automatically updated for all clients
        syncPosition = newPosition;
        syncRotation = newRotation;
    }

    // Public method to move the holdPos, callable by the local player
    public void MoveHoldPos(Vector3 newPosition, Quaternion newRotation)
    {
        if (isLocalPlayer)
        {
            // Call the command to update on the server
            CmdMoveHoldPos(newPosition, newRotation);
        }
    }
}
