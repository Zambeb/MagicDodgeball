using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Steamworks;
using System.Collections;

public class SteamKeyboardTrigger : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private TMP_InputField inputField;
    protected Callback<GamepadTextInputDismissed_t> m_GamepadTextInputDismissed;
    private bool isKeyboardOpen = false;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    private void Start()
    {
        if (SteamManager.Initialized)
        {
            m_GamepadTextInputDismissed = Callback<GamepadTextInputDismissed_t>.Create(OnGamepadTextInputDismissed);
            Debug.Log($"[SteamKeyboard] Инициализация для {gameObject.name} успешна.");
        }
        else
        {
            Debug.LogError("[SteamKeyboard] SteamManager не инициализирован! Клавиатура не будет работать.");
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Запускаем через небольшую задержку, чтобы EventSystem успела отработать
        StartCoroutine(OpenKeyboardRoutine());
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isKeyboardOpen = false;
    }

    private IEnumerator OpenKeyboardRoutine()
    {
        // Короткая пауза в 0.1 сек часто решает проблему игнорирования вызова
        yield return new WaitForSecondsRealtime(0.1f);

        if (isKeyboardOpen) yield break;

        ShowSteamOsk();
    }

    private void ShowSteamOsk()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogWarning("[SteamKeyboard] Попытка вызова клавиатуры без SteamAPI.");
            return;
        }

        isKeyboardOpen = true;
        Debug.Log($"[SteamKeyboard] Запрос на открытие клавиатуры для: {gameObject.name}");

        // Вызываем стандартную клавиатуру
        bool success = SteamUtils.ShowGamepadTextInput(
            EGamepadTextInputMode.k_EGamepadTextInputModeNormal,
            EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine,
            "Enter Text",
            20,
            inputField.text
        );

        if (success)
        {
            Debug.Log("[SteamKeyboard] Steam подтвердил вызов клавиатуры (True).");
        }
        else
        {
            Debug.LogError("[SteamKeyboard] Steam отклонил вызов (False). Проверьте, включен ли Overlay и запущен ли Big Picture.");
            
            // Альтернативный метод для плавающей клавиатуры (иногда работает лучше на новых версиях)
            // SteamUtils.ShowFloatingGamepadTextInput(EFloatingGamepadTextInputMode.k_EFloatingGamepadTextInputModeModeSingleLine, 0, 0, 0, 0);
        }
    }

    private void OnGamepadTextInputDismissed(GamepadTextInputDismissed_t callback)
    {
        isKeyboardOpen = false;
        Debug.Log($"[SteamKeyboard] Клавиатура закрыта. Submitted: {callback.m_bSubmitted}");

        if (callback.m_bSubmitted)
        {
            uint length = SteamUtils.GetEnteredGamepadTextLength();
            if (SteamUtils.GetEnteredGamepadTextInput(out string submittedText, length))
            {
                inputField.text = submittedText;
                inputField.onEndEdit.Invoke(submittedText);
                Debug.Log($"[SteamKeyboard] Текст получен: {submittedText}");
            }
        }
        
        // Возвращаем фокус на поле ввода после закрытия клавиатуры
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}