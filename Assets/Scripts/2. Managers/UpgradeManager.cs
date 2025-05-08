using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [SerializeField] private List<UpgradeData> allUpgrades;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public List<UpgradeData> GetRandomUpgrades(int count)
    {
        List<UpgradeData> randomUpgrades = new List<UpgradeData>();
        List<UpgradeData> available = new List<UpgradeData>(allUpgrades);

        for (int i = 0; i < count; i++)
        {
            if (available.Count == 0) break;
            int index = Random.Range(0, available.Count);
            randomUpgrades.Add(available[index]);
            available.RemoveAt(index);
        }

        return randomUpgrades;
    }

    public void ApplyUpgrade(PlayerController player, UpgradeData upgradeData)
    {
        var effect = upgradeData.CreateEffect();
        effect.Apply(player);
        if (effect.isActiveAbility)
        {
            player.AddActiveAbility(effect);
        }
        else
        {
            player.AddUpgrade(effect);
        }
    }
}