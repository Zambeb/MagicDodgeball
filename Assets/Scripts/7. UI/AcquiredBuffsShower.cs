using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcquiredBuffsShower : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject iconPrefab; 
    [SerializeField] private GameObject iconPrefabActive; 
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

        UpgradeEffectBase activeUpgrade = player.acquiredActiveAbility;
        List<UpgradeEffectBase> upgrades = player.acquiredUpgrades;
        
        if (activeUpgrade == null && (upgrades == null || upgrades.Count == 0)) return;

        if (activeUpgrade != null)
        {
            GameObject iconObjAct = Instantiate(iconPrefabActive, iconsParent);
            iconObjAct.transform.localScale = iconObjAct.transform.localScale / 1.5f;
            
            Image iconImageAct = iconObjAct.GetComponent<AcquiredIconImage>().iconImage.GetComponent<Image>();

            BuffIcon buffIcon = iconObjAct.GetComponent<BuffIcon>();
            if (buffIcon != null)
            {
                buffIcon.SetTooltip(activeUpgrade.upgradeName);
            }
            if (iconImageAct != null)
            {
                iconImageAct.sprite = activeUpgrade.icon;
                Color newColor;
                if (ColorUtility.TryParseHtmlString("#4E292E", out newColor))
                {
                    iconImageAct.color = newColor;
                }
                else
                {
                    Debug.LogError("wrong color code");
                }
            }
            spawnedIcons.Add(iconObjAct);
        }
        
        foreach (UpgradeEffectBase upgrade in upgrades)
        {
            GameObject iconObj = Instantiate(iconPrefab, iconsParent);
            iconObj.transform.localScale = iconObj.transform.localScale / 1.5f;
            
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