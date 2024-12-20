using UnityEngine;
using Mirror;

public class BallSpawner : NetworkBehaviour
{
    public GameObject ballPrefab; // Assign the ball prefab in the Inspector
    public Transform spawnPoint; // Assign the spawn point (empty GameObject) in the Inspector

    private void Start()
    {
        if (isServer) // Ensure this runs only on the server
        {
            Debug.Log("SPAWNED"); 
            SpawnBall();
        }
    }

    // Call this method on the server to spawn the object
    [Server]
    public void SpawnBall()
    {
        if (ballPrefab == null || spawnPoint == null)
        {
            Debug.LogError("BallPrefab or SpawnPoint is not assigned in the Inspector.");
            return;
        }

        GameObject ball = Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation); // Instantiate locally on the server
        Debug.Log("SPAWNED: Ball spawned at " + spawnPoint.position);

        NetworkServer.Spawn(ball); // Register the ball with the server so all clients see it
    }
}
