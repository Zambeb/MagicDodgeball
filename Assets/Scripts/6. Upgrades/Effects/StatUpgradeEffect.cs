using UnityEngine;

[CreateAssetMenu(fileName = "StatUpgrade", menuName = "Upgrades/StatUpgrade")]
public class StatUpgradeEffect : UpgradeEffectBase
{
    public StatType statType;
    public float value;

    public override void Apply(PlayerController player)
    {
        switch (statType)
        {
            case StatType.MoveSpeed:
                player.stats.moveSpeed += value;
                break;
            case StatType.ProjectileSpeed:
                player.stats.projectileSpeed += value;
                break;
            case StatType.MaxBounces:
                player.stats.maxBounces += (int)value;
                break;
            case StatType.MaxProjectiles:
                player.stats.maxProjectiles += (int)value;
                break;
            case StatType.AccelerationAfterBounce:
                player.stats.accelerationAfterBounce += value;
                break;
            case StatType.Stun:
                player.stats.canStun = true;
                player.stats.stunDuration = value;
                break;
            case StatType.NoSelfHarm:
                player.stats.canSelfHarm = false;
                break;
            case StatType.CanBurnArea:
                player.stats.canBurnArea = true;
                break;
            case StatType.ProjectileSize:
                player.stats.ballSizeMultiplier += value;
                break;
            default:
                Debug.LogWarning("Unknown stat type!");
                break;
        }
    }

    public override void PerformAbility(PlayerController player)
    {
        return;
    }
}