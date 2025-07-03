using UnityEngine;

[CreateAssetMenu(fileName = "ExplosiveBalls", menuName = "Upgrades/ExplosiveBalls")]

public class ExplosiveBallsEffect : UpgradeEffectBase
{
    public int miniBallsQuantity;
    public float miniBallSizeMultiplier;

    public override void Apply(PlayerController player)
    {
        player.stats.canExplodeBalls = true;
        player.stats.miniBallsQuantity = miniBallsQuantity;
        player.stats.miniBallsSizeMultiplier = miniBallSizeMultiplier;
    }

    public override void PerformAbility(PlayerController player)
    {
        return;
    }
}