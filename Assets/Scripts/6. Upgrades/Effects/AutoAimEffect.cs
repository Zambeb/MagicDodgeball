using UnityEngine;

[CreateAssetMenu(fileName = "AutoAimEffect", menuName = "Upgrades/Passive/AutoAim")]
public class AutoAimEffect : UpgradeEffectBase
{
    public float autoAimStrength;

    public override void Apply(PlayerController player)
    {
        player.stats.hasAutoAim = true;
        player.stats.autoAimStrength = autoAimStrength;
    }

    public override void PerformAbility(PlayerController player)
    {
        return;
    }
}
