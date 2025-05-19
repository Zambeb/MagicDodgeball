using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeEffectBase", menuName = "Upgrades/UpgradeEffectBase")]
public abstract class UpgradeEffectBase : ScriptableObject
{
    public string upgradeName;
    public Sprite icon;
    public bool isActiveAbility;
    public bool isStackable;
    public abstract void Apply(PlayerController player);
    public abstract void PerformAbility(PlayerController player);
}
