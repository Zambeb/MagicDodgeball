using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject playerPrefab;
    private int playerCount = 0;

    private void OnEnable()
    {
        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
    }

    private void OnDisable()
    {
        if (PlayerInputManager.instance != null)
        {
            PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
        }
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        if (playerCount >= spawnPoints.Length)
        {
            Debug.LogWarning("Not enough spawn points for all players!");
            return;
        }
        
        Transform spawnPoint = spawnPoints[playerCount];
        playerInput.transform.position = spawnPoint.position;
        playerInput.transform.rotation = spawnPoint.rotation;

        playerCount++;
    }
    
}