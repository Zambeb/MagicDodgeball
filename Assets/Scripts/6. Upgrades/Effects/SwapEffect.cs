using UnityEngine;

[CreateAssetMenu(fileName = "SwapUpgrade", menuName = "Upgrades/SwapUpgrade")]

public class SwapEffect : UpgradeEffectBase
{
    public float cooldown;

    public override void Apply(PlayerController player)
    {
        return;
    }

    public override void PerformAbility(PlayerController player)
    {
        player.Swap(cooldown);
    }
}
