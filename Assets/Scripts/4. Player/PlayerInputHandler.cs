using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public GameObject playerPrefab;
    private PlayerController playerController;

    private Vector3 startPos = new Vector3(0, 0, 0);

    private void Awake()
    {
        if (playerPrefab != null)
        {
            int playerIndex = PlayerSpawner.instance.playerList.Count - 1;
            
            playerController = GameObject.Instantiate(
                    playerPrefab, 
                    PlayerSpawner.instance.spawnPoints[playerIndex].position, 
                    transform.rotation)
                .GetComponent<PlayerController>();
            transform.parent = playerController.transform;
            transform.position = playerController.transform.position;
            playerController.playerIndex = playerIndex;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        playerController.OnMove(context);
    }
    
    public void OnAim(InputAction.CallbackContext context)
    {
        playerController.OnAim(context);
    }
    
    public void OnFire(InputAction.CallbackContext context)
    {
        playerController.OnFire(context);
    }
}
