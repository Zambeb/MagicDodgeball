using UnityEngine;

[CreateAssetMenu(fileName = "AmigaBall", menuName = "Upgrades/AmigaBall")]

public class AmigaBallEffect : UpgradeEffectBase
{
    public float projectileSpeedMultiplier;
    public int extraBounces;
    public float sizeMultiplier;
    //public Texture amigaBallTexture;

    public override void Apply(PlayerController player)
    {
        player.stats.projectileSpeed *= projectileSpeedMultiplier;
        player.stats.maxBounces += extraBounces;
        player.stats.ballSizeMultiplier += sizeMultiplier;
        player.stats.amigaBall = true;
    }

    public override void PerformAbility(PlayerController player)
    {
        return;
    }
}
