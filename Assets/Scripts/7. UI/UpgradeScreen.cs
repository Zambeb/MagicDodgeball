using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class UpgradeScreen : MonoBehaviour
{
    [SerializeField] private GameObject upgradeButtonPrefab;
    [SerializeField] private Transform buttonsParent;
    [SerializeField] private GameObject firstSelectedButton;

    private List<GameObject> spawnedButtons = new List<GameObject>();

    private PlayerController currentPlayer;

    public int buffsToChoose;
    public int buffsChosen;

    public void Open(PlayerController player, int buffs)
    {
        currentPlayer = player;
        
        buffsToChoose = buffs;
        buffsChosen = 0;

        gameObject.SetActive(true);
        GenerateUpgradeButtons();

        int playerIndex = player.playerIndex;
        string eventSystemName = $"UIInputModule_{playerIndex}";
        GameObject eventSystemObj = GameObject.Find(eventSystemName);
        if (eventSystemObj != null)
        {
            MultiplayerEventSystem ev = eventSystemObj.GetComponent<MultiplayerEventSystem>();
            if (ev != null)
            {
                ev.SetSelectedGameObject(firstSelectedButton);
            }
            else
            {
                Debug.LogWarning($"MultiplayerEventSystem not found {eventSystemName}");
            }
        }
        else
        {
            Debug.LogWarning($"Объект {eventSystemName} not found.");
        }
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
        var upgrades = UpgradeManager.Instance.GetRandomUpgrades(4, currentPlayer);

        foreach (var upgrade in upgrades)
        {
            GameObject buttonObj = Instantiate(upgradeButtonPrefab, buttonsParent);
            UpgradeButton upgradeButton = buttonObj.GetComponent<UpgradeButton>();
            upgradeButton.Setup(upgrade, this);
            spawnedButtons.Add(buttonObj);
        }

        if (spawnedButtons.Count > 0)
        {
            if (currentPlayer.currentControlScheme == "Gamepad")
            {
                firstSelectedButton = spawnedButtons[0];
            }
        }
    }

    public void SelectUpgrade(UpgradeData selectedUpgrade)
    {
        GameObject selectedButtonObj = null;
        foreach (var go in spawnedButtons)
        {
            UpgradeButton b = go.GetComponent<UpgradeButton>();
            if (b != null && b.GetUpgradeData() == selectedUpgrade)
            {
                selectedButtonObj = go;
                break;
            }
        }
        
        if (selectedButtonObj != null)
        {
            foreach (var go in spawnedButtons)
            {
                if (go != selectedButtonObj)
                    Destroy(go);
            }
            
            spawnedButtons.Clear();
            spawnedButtons.Add(selectedButtonObj);

            StartCoroutine(AnimateSelectedButton(selectedButtonObj, selectedUpgrade));
        }
        else
        {
            ApplyUpgradeAndContinue(selectedUpgrade);
        }
    }

    private IEnumerator AnimateSelectedButton(GameObject buttonObj, UpgradeData selectedUpgrade)
    {
        var buttonComponent = buttonObj.GetComponent<UnityEngine.UI.Button>();
        if (buttonComponent != null)
        {
            buttonComponent.interactable = false;
        }
        
        Vector3 startPos = buttonObj.transform.position;
        Vector3 startScale = buttonObj.transform.localScale;
        
        Vector3 targetPos = buttonsParent.position;
        
        Vector3 targetScale = startScale * 2f;

        float duration = 1f;
        float elapsed = 0f;
        
        RectTransform rt = buttonObj.GetComponent<RectTransform>();
        Vector3 startLocalPos = rt.localPosition;
        Vector3 targetLocalPos = Vector3.zero; 

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            rt.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, t);
            rt.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }
        
        yield return new WaitForSeconds(1f);
        
        ApplyUpgradeAndContinue(selectedUpgrade);
    }
    
    private void ApplyUpgradeAndContinue(UpgradeData selectedUpgrade)
    {
        UpgradeManager.Instance.ApplyUpgrade(currentPlayer, selectedUpgrade);
        buffsChosen++;

        if (buffsChosen < buffsToChoose)
        {
            ClearButtons();
            GenerateUpgradeButtons();

            if (currentPlayer.currentControlScheme == "Gamepad" && spawnedButtons.Count > 0)
            {
                int playerIndex = currentPlayer.playerIndex;
                string eventSystemName = $"UIInputModule_{playerIndex}";
                GameObject eventSystemObj = GameObject.Find(eventSystemName);
                if (eventSystemObj != null)
                {
                    MultiplayerEventSystem ev = eventSystemObj.GetComponent<MultiplayerEventSystem>();
                    if (ev != null)
                    {
                        ev.SetSelectedGameObject(spawnedButtons[0]);
                    }
                }
            }
        }
        else
        {
            RoundManager.Instance.PlayerSelectedUpgrade();
            ClearButtons();
            //Close();
        }
    }
    
    public void ChooseRandomUpgrades()
    {
        while (buffsChosen < buffsToChoose)
        {
            List<UpgradeButton> availableButtons = new List<UpgradeButton>();
            foreach (var go in spawnedButtons)
            {
                var b = go.GetComponent<UpgradeButton>();
                if (b != null)
                    availableButtons.Add(b);
            }

            if (availableButtons.Count > 0)
            {
                int randIndex = UnityEngine.Random.Range(0, availableButtons.Count);
                SelectUpgrade(availableButtons[randIndex].GetUpgradeData());
            }
            else
            {
                Debug.LogWarning("No available buffs");
                break;
            }
        }
    }

    public bool HasFinishedChoosing()
    {
        return buffsChosen >= buffsToChoose;
    }

}