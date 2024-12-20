using Mirror;
using UnityEngine;

public class BallSync : NetworkBehaviour
{
    private Rigidbody rb;

    [SyncVar] private Vector3 syncPosition;
    [SyncVar] private Quaternion syncRotation;
    [SyncVar] private Vector3 syncVelocity;
    [SyncVar] private Vector3 syncAngularVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isServer) // Server handles ball physics
        {
            // Update SyncVars with the ball's current state
            syncPosition = rb.position;
            syncRotation = rb.rotation;
            syncVelocity = rb.velocity;
            syncAngularVelocity = rb.angularVelocity;
        }
        else // Non-server clients interpolate to match the server state
        {
            // Smoothly interpolate position and rotation for visual consistency
            rb.position = Vector3.Lerp(rb.position, syncPosition, Time.deltaTime * 10);
            rb.rotation = Quaternion.Lerp(rb.rotation, syncRotation, Time.deltaTime * 10);

            // Set velocities to align with server state
            rb.velocity = syncVelocity;
            rb.angularVelocity = syncAngularVelocity;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("HERE"); 
            // Calculate the collision force based on impact and player velocity
            Vector3 collisionForce = collision.relativeVelocity * rb.mass;

            // Notify the server to apply the force without requiring authority
            CmdApplyForceWithoutAuthority(collisionForce);
        }
    }

    [Command(requiresAuthority = false)] // Allows command without client ownership
    private void CmdApplyForceWithoutAuthority(Vector3 force)
    {
        // Apply the force to the ball's rigidbody
        rb.AddForce(force, ForceMode.Impulse);

        // Update the SyncVars with the new state
        syncVelocity = rb.velocity;
        syncAngularVelocity = rb.angularVelocity;
    }
}
