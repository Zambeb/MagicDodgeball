using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    public List<UpgradeData> allUpgrades;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public List<UpgradeData> GetRandomUpgrades(int count, PlayerController player)
    {
        List<UpgradeData> result = new List<UpgradeData>();
        List<UpgradeData> available = new List<UpgradeData>(allUpgrades);

        available.RemoveAll(upgradeData =>
        {
            var effect = upgradeData.CreateEffect();
        
            if (!effect.isStackable)
            {
                bool alreadyHas = player.acquiredUpgrades.Exists(u => u.GetType() == effect.GetType());
                
                bool isActiveAndSame = effect.isActiveAbility &&
                                       player.acquiredActiveAbility != null &&
                                       player.acquiredActiveAbility.GetType() == effect.GetType();

                return alreadyHas || isActiveAndSame;
            }

            return false;
        });

        for (int i = 0; i < count; i++)
        {
            if (available.Count == 0) break;

            int index = Random.Range(0, available.Count);
            result.Add(available[index]);
            available.RemoveAt(index);
        }

        return result;
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