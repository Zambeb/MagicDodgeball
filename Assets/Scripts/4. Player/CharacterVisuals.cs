using System;
using UnityEngine;

public class CharacterVisuals : MonoBehaviour
{
    [SerializeField] private Transform modelsRoot;  
    [SerializeField] private GameObject[] characterModels;
    
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
        
        GameObject modelToEnable = characterModels[idx];
        modelToEnable.SetActive(true);
    }
}
