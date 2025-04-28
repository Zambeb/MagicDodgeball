using System.Collections.Generic;
using UnityEngine;

public class UpgradeScreen : MonoBehaviour
{
    [SerializeField] private GameObject upgradeButtonPrefab;
    [SerializeField] private Transform buttonsParent;
    private List<GameObject> spawnedButtons = new List<GameObject>();
    
    private PlayerController currentPlayer;

    public void Open(PlayerController player)
    {
        currentPlayer = player;
        gameObject.SetActive(true);
        GenerateUpgradeButtons();
    }

    private void GenerateUpgradeButtons()
    {
        var upgrades = UpgradeManager.Instance.GetRandomUpgrades(4);

        foreach (var upgrade in upgrades)
        {
            GameObject buttonObj = Instantiate(upgradeButtonPrefab, buttonsParent);
            UpgradeButton upgradeButton = buttonObj.GetComponent<UpgradeButton>();
            upgradeButton.Setup(upgrade, this);
            spawnedButtons.Add(buttonObj);
        }
    }

    public void SelectUpgrade(UpgradeData selectedUpgrade)
    {
        UpgradeManager.Instance.ApplyUpgrade(currentPlayer, selectedUpgrade);

        // Чистим всё
        foreach (var button in spawnedButtons)
        {
            Destroy(button);
        }
        spawnedButtons.Clear();

        gameObject.SetActive(false);
    }
}