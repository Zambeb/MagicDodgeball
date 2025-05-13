using System;
using System.Collections.Generic;
using UnityEngine;

public class CheatPlayerBuffs : MonoBehaviour
{
    [SerializeField] private GameObject upgradeButtonPrefab;
    [SerializeField] private Transform buttonsParent;
    public int playerNumber;
    
    private List<GameObject> spawnedButtons = new List<GameObject>();
    private PlayerController currentPlayer;

    private void Start()
    {
        if (playerNumber == 1)
        {
            currentPlayer = RoundManager.Instance.player1;
        }
        else
        {
            currentPlayer = RoundManager.Instance.player2;
        }
        GenerateUpgradeButtons();
    }
    
    private void GenerateUpgradeButtons()
    {
        var upgrades = UpgradeManager.Instance.allUpgrades;

        foreach (var upgrade in upgrades)
        {
            GameObject buttonObj = Instantiate(upgradeButtonPrefab, buttonsParent);
            CheatBuffButton upgradeButton = buttonObj.GetComponent<CheatBuffButton>();
            upgradeButton.Setup(upgrade, this);
            spawnedButtons.Add(buttonObj);
        }
    }
    
    public void SelectUpgrade(UpgradeData selectedUpgrade)
    {
        UpgradeManager.Instance.ApplyUpgrade(currentPlayer, selectedUpgrade);
        currentPlayer.ApplyAllUpgrades();
    }
}
