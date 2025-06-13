using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeEffectBase", menuName = "Upgrades/Data/UpgradeEffectBase")]
public abstract class UpgradeEffectBase : ScriptableObject
{
    public string upgradeName;
    public Sprite icon;
    public bool isActiveAbility;
    public bool isStackable;
    [ShowIf("isStackable")]
    public int maxStacks;
    public abstract void Apply(PlayerController player);
    public abstract void PerformAbility(PlayerController player);
}
