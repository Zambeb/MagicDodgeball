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
            Debug.LogError($"InputSystemUIInputModule not found in {moduleName}");
            return;
        }
        
        // Привязываем модуль к игроку
        playerInput.uiInputModule = inputModule;
        
        var actions = playerInput.actions;
        inputModule.actionsAsset = actions;

        // ВАЖНО: Здесь должно быть точное название твоей карты действий для интерфейса.
        // Обычно в Unity это "UI". Если у тебя она называется иначе (например, "Menu" или "Interface"), измени эту строку:
        string mapName = "UI"; 

        // Вспомогательная локальная функция для безопасного поиска
        InputAction FindActionSafe(string actionName)
        {
            var action = actions.FindAction($"{mapName}/{actionName}");
            if (action == null) 
                Debug.LogWarning($"Внимание: Не найдено действие {mapName}/{actionName} в Action Asset!");
            return action;
        }

        // 3. Заполняем поля жесткими ссылками, чтобы они больше не были None
        inputModule.move = InputActionReference.Create(FindActionSafe("Navigate"));
        inputModule.submit = InputActionReference.Create(FindActionSafe("Submit"));
        inputModule.cancel = InputActionReference.Create(FindActionSafe("Cancel"));
        inputModule.point = InputActionReference.Create(FindActionSafe("Point"));
        inputModule.leftClick = InputActionReference.Create(FindActionSafe("Click"));
        inputModule.scrollWheel = InputActionReference.Create(FindActionSafe("ScrollWheel"));
        inputModule.middleClick = InputActionReference.Create(FindActionSafe("MiddleClick"));
        inputModule.rightClick = InputActionReference.Create(FindActionSafe("RightClick"));
        inputModule.trackedDevicePosition = InputActionReference.Create(FindActionSafe("TrackedDevicePosition"));
        inputModule.trackedDeviceOrientation = InputActionReference.Create(FindActionSafe("TrackedDeviceOrientation"));
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
