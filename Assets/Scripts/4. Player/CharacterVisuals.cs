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
    }
    
    public void FlashWhite(int flashes, float totalDuration)
    {
        StartCoroutine(FlashWhiteCoroutine(flashes, totalDuration));
    }
    
    private IEnumerator FlashWhiteCoroutine(int flashes, float totalDuration)
    {
        if (model == null)
            yield break;

        Renderer renderer = model.GetComponentInChildren<Renderer>();
        if (renderer == null)
            yield break;

        Material originalMaterial = renderer.material;
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
}
