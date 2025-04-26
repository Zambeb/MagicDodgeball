using UnityEngine;

[CreateAssetMenu(fileName = "MoveSpeedUpgrade", menuName = "Upgrades/MoveSpeedUpgrade")]
public class MoveSpeedUpgradeEffect : UpgradeEffectBase
{
    public float speedIncrease = 1.5f;

    public override void Apply(PlayerController player)
    {
        player.stats.moveSpeed += speedIncrease;
    }
}