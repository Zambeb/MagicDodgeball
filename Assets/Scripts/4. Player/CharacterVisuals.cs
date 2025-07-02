using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVisuals : MonoBehaviour
{
    [SerializeField] private Transform modelsRoot;  
    [SerializeField] private GameObject[] characterModels;
    
    [SerializeField] private Material whiteMaterial;
    public GameObject model;
    private Renderer renderer;
    private Material originalMaterial;
    
    private Coroutine flashCoroutine;
    private Coroutine chargingVFXCoroutine;

    [Header("VFX")] 
    [SerializeField] private GameObject ParryVFX;
    [SerializeField] private GameObject swapVFX;
    [SerializeField] private GameObject stunVFX;
    [SerializeField] private GameObject dashVFX;
    [SerializeField] private GameObject forceFieldVFX;
    [SerializeField] private GameObject immunityVFX;
    [SerializeField] private GameObject chargingVFX;
    
    private void Start()
    {
        var controller = GetComponent<PlayerController>();
        if (controller == null)
        {
            Debug.LogWarning("PlayerController not found on " + name);
            return;
        }

        int idx = controller.playerIndex;
        if (idx < 0 || idx >= characterModels.Length)
        {
            Debug.LogWarning($"No character model for playerIndex {idx}");
            return;
        }
        
        foreach (Transform child in modelsRoot)
        {
            child.gameObject.SetActive(false);
        }
        
        model = characterModels[idx];
        model.SetActive(true);
        renderer = model.GetComponentInChildren<Renderer>();
        originalMaterial = renderer.material;
    }
    
    public void FlashWhite(int flashes, float totalDuration)
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            renderer.material = originalMaterial; 
        }
        flashCoroutine = StartCoroutine(FlashWhiteCoroutine(flashes, totalDuration));
    }
    
    private IEnumerator FlashWhiteCoroutine(int flashes, float totalDuration)
    {
        if (model == null)
            yield break;
        
        if (renderer == null)
            yield break;
        
        float flashDuration = totalDuration / (flashes * 2);

        for (int i = 0; i < flashes; i++)
        {
            renderer.material = whiteMaterial;
            yield return new WaitForSeconds(flashDuration);

            renderer.material = originalMaterial;
            yield return new WaitForSeconds(flashDuration);
        }
        
        renderer.material = originalMaterial;
    }

    public void ParryVisualEffect()
    {
        Vector3 spawnPosition = transform.position;
        spawnPosition.y = 1f; 

        GameObject effect = Instantiate(ParryVFX, spawnPosition, Quaternion.identity);
        effect.transform.localScale *= 4f; 
        
        Destroy(effect, 1f);
    }

    public GameObject SwapVisualEffect()
    {
        Vector3 spawnPosition = transform.position;
        spawnPosition.y = 0.2f;
        GameObject effect = Instantiate(swapVFX, spawnPosition, Quaternion.identity);
        effect.transform.localScale *= 0.5f;
        Destroy(effect, 2f);
        
        return effect;
    }

    public void StunVisualEffect(float duration)
    {
        Vector3 spawnPosition = transform.position;
        spawnPosition.y = 2f;
        GameObject effect = Instantiate(stunVFX, spawnPosition, Quaternion.identity);
        effect.transform.localScale *= 0.5f; 
        
        Destroy(effect, duration);
    }

    public void DashVisualEffect(Vector3 dashDirection, float duration)
    {
        Vector3 spawnPosition = transform.position;
        spawnPosition.y = 1f;
        
        GameObject effect = Instantiate(dashVFX, spawnPosition, Quaternion.identity);
        
        effect.transform.SetParent(transform);
        effect.transform.localPosition = new Vector3(0, 1f, 0);
        
        Vector3 localDashDir = transform.InverseTransformDirection(dashDirection);
        localDashDir.y = 0;
        localDashDir.Normalize();
        float angle = Mathf.Atan2(localDashDir.x, localDashDir.z) * Mathf.Rad2Deg;
        
        effect.transform.localRotation = Quaternion.Euler(-90f, 0, angle);
        effect.transform.localScale *= 0.3f; 
    
        Destroy(effect, duration);
    }

    public void ForceFieldEffect(float duration)
    {
        Vector3 spawnPosition = transform.position;
        //spawnPosition.y = 1f;
        
        GameObject effect = Instantiate(forceFieldVFX, spawnPosition, Quaternion.identity);
        
        effect.transform.SetParent(transform);
        //effect.transform.localPosition = new Vector3(0, 1f, 0);
        
        Destroy(effect, duration);
    }
    
    public void ImmunityFVX(float duration)
    {
        Vector3 spawnPosition = transform.position;
        //spawnPosition.y = 1f;
        
        GameObject effect = Instantiate(immunityVFX, spawnPosition, Quaternion.identity);
        
        effect.transform.SetParent(transform);
        //effect.transform.localPosition = new Vector3(0, 1f, 0);
        
        Destroy(effect, duration);
    }
    
    public void ChargingVFXOn(float duration)
    {
        GameObject instance = Instantiate(chargingVFX, transform.position, Quaternion.identity);
        instance.transform.SetParent(transform);
        
        //instance.transform.localScale = Vector3.one * 0.1f;
        
        //StartCoroutine(ScaleChargingVFX(instance.transform, duration));
    }

    private IEnumerator ScaleChargingVFX(Transform effectTransform, float duration)
    {
        Vector3 startScale = Vector3.one * 0.1f;
        Vector3 endScale = Vector3.one;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            effectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        effectTransform.localScale = endScale;
    }
    
    public void ChargingVFXOff()
    {
        foreach (Transform child in transform)
        {
            if (child.name.Contains(chargingVFX.name))
            {
                Destroy(child.gameObject);
            }
        }
    }
}
