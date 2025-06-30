using UnityEngine;
using UnityEngine.UI;

public class CooldownRing : MonoBehaviour
{
    public Image ringImage;
    //public float hideDelay = 1f;

    private float remainingCooldown;
    private float totalCooldown;
    private bool isCooldownActive = false;
    //private bool isVisible = false;
    //private float hideTimer;
    
    private void Start()
    {
        ringImage.fillAmount = 0f;
        EndCooldown();
    }

    public void StartCooldown(float total)
    {
        totalCooldown = total;
        remainingCooldown = total;
        isCooldownActive = true;

        ringImage.fillAmount = 1f; 
        gameObject.SetActive(true);
        ringImage.SetAllDirty();
        
        if (ringImage.canvasRenderer != null)
        {
            ringImage.canvasRenderer.cull = false;
            Canvas.ForceUpdateCanvases();
        }
    }

    private void Update()
    {
        if (!isCooldownActive)
            return;

        remainingCooldown -= Time.deltaTime;

        float fill = Mathf.Clamp01(remainingCooldown / totalCooldown);
        ringImage.fillAmount = fill;

        if (remainingCooldown <= 0f)
        {
            EndCooldown();
        }
        
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    private void EndCooldown()
    {
        isCooldownActive = false;
        gameObject.SetActive(false);
    }

}