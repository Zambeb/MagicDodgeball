using UnityEngine;

[CreateAssetMenu(fileName = "KnockBackEffect", menuName = "Upgrades/Passive/KnockBack")]
public class KnockBackEffect : UpgradeEffectBase
{
    public float knockBackDistance;
    public float knockBackDuration;

    public override void Apply(PlayerController player)
    {
        player.stats.knockBack = true;
        player.stats.knockBackDistance = knockBackDistance;
        player.stats.knockBackDuration = knockBackDuration;
    }

    public override void PerformAbility(PlayerController player)
    {
        return;
    }
}
