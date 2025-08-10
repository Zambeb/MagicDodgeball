using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadRumble : MonoBehaviour
{
    [Header("GetHit")] 
    [SerializeField] private float getHitLF = 0.5f;
    [SerializeField] private float getHitHF = 0.5f;
    [SerializeField] private float getHitDur = 0.5f;
    
    [Header("Shoot")]
    [SerializeField] private float shootLF = 0;
    [SerializeField] private float shootHF = 0.5f;
    [SerializeField] private float shootDur = 0.5f;
    
    private Coroutine chargingCoroutine;
    private Gamepad gamepad;
    private PlayerInput playerInput;
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        UpdateGamepadReference();
        
        playerInput.onControlsChanged += OnControlsChanged;
    }

    private void OnControlsChanged(PlayerInput input)
    {
        UpdateGamepadReference();
    }

    private void UpdateGamepadReference()
    {
        var device = playerInput.GetDevice<InputDevice>();
        
        gamepad = device as Gamepad;
        
        if (gamepad == null && device != null)
        {
            Debug.Log($"Устройство ввода {device.name} не является геймпадом", this);
        }
    }

    private void Vibrate(float lf, float hf, float dur)
    {
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(lf, hf);
            Invoke(nameof(StopVibration), dur);
        }
        else
        {
            Debug.Log("Геймпад не назначен для этого игрока.");
        }
    }
    
    public void ShootRumble()
    {
        Vibrate(shootLF, shootHF, shootDur);
    }
    
    public void GetHitRumble()
    {
        Vibrate(getHitLF, getHitHF, getHitDur);
    }

    private void StopVibration()
    {
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0, 0);
        }
    }
    
    public void ChargingRumble(float maxLF, float maxHF, float chargeTime)
    {
        if (chargingCoroutine != null)
            StopCoroutine(chargingCoroutine);

        chargingCoroutine = StartCoroutine(ChargingRumbleCoroutine(maxLF, maxHF, chargeTime));
    }

    private IEnumerator ChargingRumbleCoroutine(float maxLF, float maxHF, float chargeTime)
    {
        if (gamepad == null)
        {
            Debug.Log("Геймпад не назначен для этого игрока.");
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < chargeTime)
        {
            float t = elapsed / chargeTime;  
            float currentLF = Mathf.Lerp(0, maxLF, t);
            float currentHF = Mathf.Lerp(0, maxHF, t);

            gamepad.SetMotorSpeeds(currentLF, currentHF);

            elapsed += Time.deltaTime;
            yield return null;
        }
        
        gamepad.SetMotorSpeeds(maxLF, maxHF);
    }

    public void StopChargingRumble()
    {
        if (chargingCoroutine != null)
        {
            StopCoroutine(chargingCoroutine);
            chargingCoroutine = null;
        }
        StopVibration();
    }
    
    public void FadeOutRumble(float startLF, float startHF, float fadeDuration)
    {
        StartCoroutine(FadeOutRumbleCoroutine(startLF, startHF, fadeDuration));
    }

    private IEnumerator FadeOutRumbleCoroutine(float startLF, float startHF, float fadeDuration)
    {
        if (gamepad == null)
        {
            Debug.Log("Геймпад не назначен для этого игрока.");
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;
            float currentLF = Mathf.Lerp(startLF, 0, t);
            float currentHF = Mathf.Lerp(startHF, 0, t);

            gamepad.SetMotorSpeeds(currentLF, currentHF);

            elapsed += Time.deltaTime;
            yield return null;
        }

        StopVibration();
    }
    
    private void OnDestroy()
    {
        if (playerInput != null)
            playerInput.onControlsChanged -= OnControlsChanged;
            
        StopVibration();
    }

    private void OnDisable()
    {
        StopVibration();
    }
}