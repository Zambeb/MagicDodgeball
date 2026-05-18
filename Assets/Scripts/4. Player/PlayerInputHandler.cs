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
            var playerInput = GetComponent<PlayerInput>();
            
            int playerIndex = playerInput.playerIndex;
            
            playerController = GameObject.Instantiate(
                    playerPrefab, 
                    PlayerSpawner.instance.spawnPoints[playerIndex].position, 
                    transform.rotation)
                .GetComponent<PlayerController>();
            transform.parent = playerController.transform;
            transform.position = playerController.transform.position;
            playerController.playerIndex = playerIndex;
            
            AssignUIInputModule(playerIndex, playerInput);
        }
    }

    private void AssignUIInputModule(int playerIndex, PlayerInput playerInput)
    {
        GameObject eventSystemsObj = GameObject.Find("EventSystems");
        if (eventSystemsObj == null) return;
        
        string moduleName = $"UIInputModule_{playerIndex}";
        Transform moduleTransform = eventSystemsObj.transform.Find(moduleName);
        if (moduleTransform == null) return;
        
        var inputModule = moduleTransform.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        if (inputModule == null) return;

        // 1. Привязываем модуль к клонированным экшенам игрока. 
        // Это сохраняет изоляцию устройств (каждый геймпад управляет только своим модулем).
        inputModule.actionsAsset = playerInput.actions;
        
        // ВАЖНО: Мы НЕ пишем playerInput.uiInputModule = inputModule; 
        // Именно встроенная логика PlayerInput сбрасывала всё в None!
        
        // 2. Перманентно включаем карту интерфейса. 
        // Теперь навигация (Navigate) будет работать всегда, когда открыто любое меню.
        var uiMap = playerInput.actions.FindActionMap("UI");
        if (uiMap != null)
        {
            uiMap.Enable();
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

    public void OnPerformActiveAbility(InputAction.CallbackContext context)
    {
        playerController.OnPerformActiveAbility(context);
    }
    
    public void OnPause(InputAction.CallbackContext context)
    {
        Debug.Log($"Игрок {playerController.playerIndex} нажал кнопку паузы. Статус: {context.phase}");
        if (context.started)
        {
            if (PauseManager.Instance.IsPaused)
            {
                PauseManager.Instance.Resume();
            }
            else
            {
                // Передаем индекс, чтобы выдать фокус конкретному геймпаду
                PauseManager.Instance.Pause(playerController.playerIndex);
            }
        }
    }
}
