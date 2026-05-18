using UnityEngine;
using Steamworks; // Для оверлея
using UnityEngine.InputSystem; // Для отслеживания геймпадов

public class SystemEventsHandler : MonoBehaviour
{
    protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;

    private void Start()
    {
        // 1. Подписка на события оверлея Steam
        if (SteamManager.Initialized)
        {
            m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
        }

        // 2. Подписка на события устройств (Input System)
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    // Вызывается, когда Steam Overlay открывается или закрывается
    private void OnGameOverlayActivated(GameOverlayActivated_t callback)
    {
        // m_bActive != 0 означает, что оверлей только что открылся
        if (callback.m_bActive != 0) 
        {
            Debug.Log("[SystemEvents] Steam Overlay открыт. Ставим на паузу.");
            TriggerAutoPause();
        }
        
        // Важное правило UX: когда оверлей ЗАКРЫВАЕТСЯ, мы НЕ снимаем игру с паузы автоматически.
        // Игрок должен сам нажать "Resume", чтобы успеть положить руки на геймпад.
    }

    // Вызывается при любом изменении оборудования (подключили мышь, отключили геймпад и т.д.)
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        // Нас интересует только отключение и только геймпадов
        if (change == InputDeviceChange.Disconnected && device is Gamepad)
        {
            Debug.Log($"[SystemEvents] Геймпад отключен: {device.name}. Ставим на паузу.");
            TriggerAutoPause();
        }
    }

    private void TriggerAutoPause()
    {
        // Проверяем, существует ли PauseManager и не на паузе ли мы уже
        if (PauseManager.Instance != null && !PauseManager.Instance.IsPaused)
        {
            // Передаем 0 (Индекс Игрока 1). 
            // Так как событие системное, отдаем контроль над меню паузы первому игроку по умолчанию.
            PauseManager.Instance.Pause(0); 
        }
    }

    private void OnDestroy()
    {
        // Обязательно отписываемся от событий инпута при уничтожении объекта, чтобы избежать утечек памяти
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
}