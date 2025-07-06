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

    private void Vibrate(float lf, float hf, float dur)
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(lf, hf);
            Invoke(nameof(StopVibration), dur);
        }
        else
        {
            Debug.Log("Геймпад не подключён.");
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
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0, 0);
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
        if (Gamepad.current == null)
        {
            Debug.Log("Геймпад не подключён.");
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < chargeTime)
        {
            float t = elapsed / chargeTime;  
            float currentLF = Mathf.Lerp(0, maxLF, t);
            float currentHF = Mathf.Lerp(0, maxHF, t);

            Gamepad.current.SetMotorSpeeds(currentLF, currentHF);

            elapsed += Time.deltaTime;
            yield return null;
        }
        
        Gamepad.current.SetMotorSpeeds(maxLF, maxHF);
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
}