using UnityEngine;

[CreateAssetMenu(fileName = "ShieldUpgrade", menuName = "Upgrades/ShieldUpgrade")]
public class ShieldUpgradeEffect : UpgradeEffectBase
{
    public override void Apply(PlayerController player)
    {
        player.shieldOrbit.AddShield();
    }

    public override void PerformAbility(PlayerController player)
    {
        return;
    }
}
