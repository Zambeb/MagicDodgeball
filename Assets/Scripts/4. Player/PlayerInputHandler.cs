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
            
            AssignUIInputModule(playerIndex);
        }
    }

    private void AssignUIInputModule(int playerIndex)
    {
        GameObject eventSystemsObj = GameObject.Find("EventSystems");
        if (eventSystemsObj == null)
        {
            Debug.LogError("EventSystems not found!");
            return;
        }
        
        string moduleName = $"UIInputModule_{playerIndex}";
        Transform moduleTransform = eventSystemsObj.transform.Find(moduleName);
        if (moduleTransform == null)
        {
            Debug.LogError($"Not found UI Input Module: {moduleName}");
            return;
        }
        
        var inputModule = moduleTransform.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        if (inputModule == null)
        {
            Debug.LogError($"InputSystemUIInputModule not found in {moduleName} ");
            return;
        }
        
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput not found in PlayerInputHandler!");
            return;
        }

        playerInput.uiInputModule = inputModule;
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

    public void OnPerformActiveAbility(InputAction.CallbackContext context)
    {
        playerController.OnPerformActiveAbility(context);
    }
    
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (PauseManager.Instance.IsPaused)
                PauseManager.Instance.Resume();
            else
                PauseManager.Instance.Pause();
        }
    }
}
