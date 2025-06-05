using UnityEngine;
using UnityEngine.UI;

public class CooldownRing : MonoBehaviour
{
    public Image ringImage;
    public float hideDelay = 1f;

    private float currentCooldown;
    private float totalCooldown;
    private bool isVisible = false;
    private float hideTimer;

    public void ShowCooldown(float remaining, float total)
    {
        currentCooldown = remaining;
        totalCooldown = total;
        ringImage.fillAmount = currentCooldown / totalCooldown;
        hideTimer = hideDelay;

        if (!isVisible)
        {
            gameObject.SetActive(true);
            isVisible = true;
        }
    }

    private void Update()
    {
        if (!isVisible) return;

        currentCooldown -= Time.deltaTime;
        float fill = 1f - Mathf.Clamp01(currentCooldown / totalCooldown);
        ringImage.fillAmount = fill;
        
        if (fill >= 1f)
        {
            Hide();
            return;
        }

        hideTimer -= Time.deltaTime;
        if (hideTimer <= 0f)
        {
            Hide();
        }
        
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    private void Hide()
    {
        isVisible = false;
        gameObject.SetActive(false);
    }

}