using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeScreen : MonoBehaviour
{
    [SerializeField] private GameObject upgradeButtonPrefab;
    [SerializeField] private Transform buttonsParent;
    [SerializeField] private GameObject firstSelectedButton;
    
    private List<GameObject> spawnedButtons = new List<GameObject>();
    
    private PlayerController currentPlayer;

    public void Open(PlayerController player)
    {
        currentPlayer = player;
        gameObject.SetActive(true);
        GenerateUpgradeButtons();
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }
    
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void ClearButtons()
    {
        foreach (var button in spawnedButtons)
        {
            Destroy(button);
        }
        spawnedButtons.Clear();
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
        
        if (spawnedButtons.Count > 0)
        {
            firstSelectedButton = spawnedButtons[0];
        }
    }

    public void SelectUpgrade(UpgradeData selectedUpgrade)
    {
        UpgradeManager.Instance.ApplyUpgrade(currentPlayer, selectedUpgrade);
        RoundManager.Instance.PlayerSelectedUpgrade();
        ClearButtons();
    }
}