using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    public List<UpgradeData> allUpgrades;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  
            return;
        }
        Instance = this;
        
        Random.InitState(System.DateTime.Now.Millisecond);
    }

    public List<UpgradeData> GetRandomUpgrades(int count, PlayerController player)
    {
        List<UpgradeData> result = new List<UpgradeData>();
        List<UpgradeData> available = new List<UpgradeData>(allUpgrades);

        available.RemoveAll(upgradeData =>
        {
            //var effect = upgradeData.CreateEffect();
            var effectPrefab = upgradeData.effectPrefab;
        
            if (!effectPrefab.isStackable)
            {
                bool alreadyHas = player.acquiredUpgradeEffectsPrefabs.Contains(effectPrefab);

                bool isActiveAndSame = effectPrefab.isActiveAbility &&
                                       player.acquiredActiveAbilityPrefab != null &&
                                       player.acquiredActiveAbilityPrefab == effectPrefab;

                return alreadyHas || isActiveAndSame;
            }
            int stackCount = 0;
            foreach (var buff in player.acquiredUpgradeEffectsPrefabs)
            {
                if (buff == effectPrefab)
                    stackCount++;
            }

            return stackCount >= effectPrefab.maxStacks;
        });

        // Fisher-Yates shuffle
        for (int i = 0; i < available.Count; i++)
        {
            int randomIndex = Random.Range(i, available.Count);
            (available[i], available[randomIndex]) = (available[randomIndex], available[i]); // Swap via deconstruction
        }
        
        for (int i = 0; i < count && i < available.Count; i++)
        {
            result.Add(available[i]);
        }

        return result;
    }

    public void ApplyUpgrade(PlayerController player, UpgradeData upgradeData)
    {
        var effect = upgradeData.CreateEffect();
        effect.Apply(player);
        if (effect.isActiveAbility)
        {
            player.acquiredActiveAbility = effect;
            player.acquiredActiveAbilityPrefab = upgradeData.effectPrefab;
        }
        else
        {
            player.acquiredUpgrades.Add(effect);
            player.acquiredUpgradeEffectsPrefabs.Add(upgradeData.effectPrefab);
        }
    }
}