using UnityEngine;

[CreateAssetMenu(fileName = "DoubleShotEffect", menuName = "Upgrades/Passive/DoubleShot")]

public class DoubleShotEffect : UpgradeEffectBase
{
    public float doubleShotChance;
    public float doubleShotInterval;

    public override void Apply(PlayerController player)
    {
        player.stats.doubleShot = true;
        player.stats.doubleShotChance += doubleShotChance;
        player.stats.doubleShotInterval = doubleShotInterval;
    }

    public override void PerformAbility(PlayerController player)
    {
        return;
    }
}
