using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public List<PlayerInput> playerList = new List<PlayerInput>();
    [SerializeField] private InputAction joinAction;
    
    // Instances
    public static PlayerSpawner instance = null;
    
    // Events
    public event System.Action<PlayerInput> PlayerJoinedGame; 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);
        
        joinAction.Enable();
        joinAction.performed += context => JoinAction(context);
    }

    private void Start()
    {
        PlayerInputManager.instance.JoinPlayer(0, -1, null);
    }

    void OnPlayerJoined(PlayerInput playerInput)
    {
        playerList.Add(playerInput);
        
        int playerIndex = playerList.Count - 1;
        
        if (playerIndex < spawnPoints.Length)
        {
            playerInput.transform.position = spawnPoints[playerIndex].position;
        }
        
        
        
        if (PlayerJoinedGame != null)
        {
            PlayerJoinedGame(playerInput);
        }
    }

    private void JoinAction(InputAction.CallbackContext context)
    {
        PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(context);
    }
}