using UnityEngine;

[CreateAssetMenu(fileName = "SlowingTrail", menuName = "Upgrades/SlowingTrail")]

public class SlowingTrailEffect : UpgradeEffectBase
{
    public float trailDuration;
    public float slowAmount;

    public override void Apply(PlayerController player)
    {
        player.stats.leavesTrail = true;
        player.stats.trailDuration = trailDuration;
        player.stats.slowAmount = slowAmount;
    }

    public override void PerformAbility(PlayerController player)
    {
        return;
    }

}
