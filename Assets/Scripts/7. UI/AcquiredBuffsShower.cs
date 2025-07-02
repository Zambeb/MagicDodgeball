using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcquiredBuffsShower : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject iconPrefab; 
    [SerializeField] private Transform iconsParent; 

    private List<GameObject> spawnedIcons = new List<GameObject>();
    

    private void Start()
    {
    }

    public void UpdateBuffIcons()
    {
        foreach (GameObject icon in spawnedIcons)
        {
            Destroy(icon);
        }
        spawnedIcons.Clear();

        PlayerController player;
        
        if (playerIndex == 0)
        {
            player = RoundManager.Instance.player1;
        }
        else if (playerIndex == 1)
        {
            player = RoundManager.Instance.player2;
        }
        else
        {
            Debug.LogError($"Unknown playerIndex: {playerIndex}");
            return;
        }
        
        List<UpgradeEffectBase> upgrades = player.acquiredUpgrades;
        
        if (upgrades == null || upgrades.Count == 0) return;
        
        foreach (UpgradeEffectBase upgrade in upgrades)
        {
            GameObject iconObj = Instantiate(iconPrefab, iconsParent);
            Image iconImage = iconObj.GetComponent<AcquiredIconImage>().iconImage.GetComponent<Image>();

            BuffIcon buffIcon = iconObj.GetComponent<BuffIcon>();
            if (buffIcon != null)
            {
                buffIcon.SetTooltip(upgrade.upgradeName);
            }
            if (iconImage != null)
            {
                iconImage.sprite = upgrade.icon;
            }
            spawnedIcons.Add(iconObj);
        }
    }
}