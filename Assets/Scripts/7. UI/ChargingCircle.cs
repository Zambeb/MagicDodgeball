using UnityEngine;
using UnityEngine.UI;

public class ChargingCircle : MonoBehaviour
{
    public Image circleImage;
    public float shakeIntensity = 5f; 
    public float shakeSpeed = 50f; 
    
    private float currentCharge;
    private float totalCharge;
    private bool isCharging;
    private bool isShaking;
    private Vector3 originalLocalPosition; 
    private Quaternion originalRotation;

    private void Awake()
    {
        if (circleImage != null)
            circleImage.fillAmount = 0;
        
        originalLocalPosition = transform.localPosition;
        originalLocalPosition.y = 321f;
        originalRotation = transform.rotation;
        gameObject.SetActive(false);
    }
    
    public void Show(float duration)
    {
        gameObject.SetActive(true);
        
        transform.localPosition = originalLocalPosition;
        transform.rotation = originalRotation;
        
        currentCharge = 0;
        totalCharge = duration;
        circleImage.fillAmount = 0;
        isShaking = false;

        isCharging = true;
    }

    private void Update()
    {
        if (!isCharging) return;
        
        currentCharge += Time.deltaTime;
        float fill = Mathf.Clamp01(currentCharge / totalCharge);
        circleImage.fillAmount = fill;
        
        if (!isShaking && Camera.main != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
        
        if (fill >= 1f && !isShaking)
        {
            StartShaking();
        }
        
        if (isShaking)
        {
            float shakeOffset = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity;
            transform.localPosition = originalLocalPosition + new Vector3(shakeOffset, shakeOffset, 0);
        }
    }
    
    public void Hide()
    {
        isCharging = false;
        isShaking = false;
        transform.localPosition = originalLocalPosition;
        transform.rotation = originalRotation;
        gameObject.SetActive(false);
    }
    
    private void StartShaking()
    {
        isShaking = true;
    }
}