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

    private bool isAnimating = false;
    private int activeAnimations = 0;

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
        TooltipUI.Instance.Hide();
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
        if (buffsChosen >= buffsToChoose) return;
        var upgrades = UpgradeManager.Instance.GetRandomUpgrades(4, currentPlayer);

        foreach (var upgrade in upgrades)
        {
            GameObject buttonObj = Instantiate(upgradeButtonPrefab, buttonsParent);
            UpgradeButton upgradeButton = buttonObj.GetComponent<UpgradeButton>();
            upgradeButton.Setup(upgrade, this, currentPlayer);
            spawnedButtons.Add(buttonObj);
        }

        if (spawnedButtons.Count > 0 && currentPlayer.currentControlScheme == "Gamepad")
        {
            firstSelectedButton = spawnedButtons[0];
        }
    }

    public void SelectUpgrade(UpgradeData selectedUpgrade)
    {
        if (isAnimating) return;

        foreach (var go in spawnedButtons)
        {
            var button = go.GetComponent<UpgradeButton>();
            if (button != null && button.upgradeData == selectedUpgrade)
            {
                StartCoroutine(AnimateAndApply(button.gameObject, selectedUpgrade));
                return;
            }
        }

        Debug.LogWarning("Selected upgrade button not found");
    }

    public void ChooseRandomUpgrades()
    {
        if (isAnimating || buffsChosen >= buffsToChoose) return;

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
            StartCoroutine(AnimateAndApply(availableButtons[randIndex].gameObject, availableButtons[randIndex].upgradeData));
        }
        else
        {
            Debug.LogWarning("No available buffs");
        }
    }

    public bool HasFinishedChoosing()
    {
        return buffsChosen >= buffsToChoose && !isAnimating;
    }
    
    public bool IsAnimating()
    {
        return isAnimating || activeAnimations > 0;
    }

    private IEnumerator AnimateAndApply(GameObject button, UpgradeData upgrade)
    {
        isAnimating = true;
        activeAnimations++;
        
        UIManager.Instance.PauseUpgradeTimer();
        
        UpgradeManager.Instance.ApplyUpgrade(currentPlayer, upgrade);
        UIManager.Instance.UpdateAllAcquiredBuffs();

        button.GetComponent<UpgradeButton>().StopWobble();
        Vector3 originalPos = button.transform.position;
        RectTransform parentRect = buttonsParent.GetComponent<RectTransform>();
        Vector3 targetLocalPos = Vector3.zero;
        Vector3 targetWorldPos = parentRect.TransformPoint(targetLocalPos);
        Vector3 targetPos = targetWorldPos;

        Vector3 originalScale = button.transform.localScale;
        float targetScaleMultiplier = 1.5f;

        RectTransform rt = button.GetComponent<RectTransform>();

        float duration = 1f;
        float elapsed = 0f;

        foreach (var go in spawnedButtons)
        {
            if (go != button)
            {
                Destroy(go);
            }
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            rt.position = Vector3.Lerp(originalPos, targetPos, t);

            // Синус с замедленным уменьшением
            float sinValue;

            if (t <= 0.5f)
            {
                // Резкий рост
                sinValue = Mathf.Sin(Mathf.PI * t);
            }
            else
            {
                float progress = (t - 0.5f) * 2f; // 0..1
                float eased = progress * progress; // медленный старт, быстрый конец
                float adjustedT = 0.5f + eased * 0.5f;
                sinValue = Mathf.Sin(Mathf.PI * adjustedT);
            }

            float scaleMultiplier = 1f + (targetScaleMultiplier - 1f) * sinValue;
            rt.localScale = originalScale * scaleMultiplier;

            yield return null;
        }

        // Визуальный эффект
        //PlayUpgradeVFX(rt.position);

        yield return new WaitForSeconds(0.5f);

        Destroy(button);
        spawnedButtons.Clear();
        
        buffsChosen++;

        if (buffsChosen < buffsToChoose)
        {
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
        activeAnimations--;
        isAnimating = false;
        UIManager.Instance.ResumeUpgradeTimer();

        if (buffsChosen >= buffsToChoose)
        {
            RoundManager.Instance.PlayerSelectedUpgrade();
        }
    }
}
