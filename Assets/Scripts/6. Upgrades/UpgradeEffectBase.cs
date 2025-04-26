using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeEffectBase", menuName = "Upgrades/UpgradeEffectBase")]
public abstract class UpgradeEffectBase : ScriptableObject
{
    public abstract void Apply(PlayerController player);
}
