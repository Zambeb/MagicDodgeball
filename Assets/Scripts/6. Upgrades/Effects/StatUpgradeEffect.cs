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
            case StatType.MaxHealth:
                player.stats.maxHealth += (int)value;
                player.stats.currentHealth += (int)value;
                UIManager.Instance.UpdatePlayerHearts(player.playerIndex, player.stats.currentHealth); 
                break;
            case StatType.MaxBounces:
                player.stats.maxBounces += (int)value;
                break;
            case StatType.MaxProjectiles:
                player.stats.maxProjectiles += (int)value;
                break;
            default:
                Debug.LogWarning("Unknown stat type!");
                break;
        }
    }
}