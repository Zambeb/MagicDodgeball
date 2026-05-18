using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Steamworks; // Требуется установленный Steamworks.NET

public class SteamKeyboardTrigger : MonoBehaviour, ISelectHandler
{
    private TMP_InputField inputField;
    protected Callback<GamepadTextInputDismissed_t> m_GamepadTextInputDismissed;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    private void Start()
    {
        if (SteamManager.Initialized)
        {
            // Регистрируем колбэк: что делать, когда клавиатура закроется
            m_GamepadTextInputDismissed = Callback<GamepadTextInputDismissed_t>.Create(OnGamepadTextInputDismissed);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Проверяем, что ввод идет с контроллера (а не мышкой)
        // Если Steam просит Full Controller Support, клавиатура должна открываться при навигации геймпадом
        ShowSteamOsk();
    }

    private void ShowSteamOsk()
    {
        if (!SteamManager.Initialized) return;

        // Вызываем клавиатуру Steam
        // Параметры: режим (нормальный), тип (одна строка), описание, макс. символов, текущий текст
        bool opened = SteamUtils.ShowGamepadTextInput(
            EGamepadTextInputMode.k_EGamepadTextInputModeNormal,
            EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine,
            "Enter Player Name",
            20,
            inputField.text
        );

        if (opened)
        {
            Debug.Log("Steam OSK Opened");
        }
    }

    private void OnGamepadTextInputDismissed(GamepadTextInputDismissed_t callback)
    {
        if (callback.m_bSubmitted)
        {
            // Получаем текст, который ввел пользователь в оверлее
            uint length = SteamUtils.GetEnteredGamepadTextLength();
            SteamUtils.GetEnteredGamepadTextInput(out string submittedText, length);

            inputField.text = submittedText;
            
            // Вызываем событие OnEndEdit вручную, чтобы MainMenu подхватил имя
            inputField.onEndEdit.Invoke(submittedText);
        }
    }
}