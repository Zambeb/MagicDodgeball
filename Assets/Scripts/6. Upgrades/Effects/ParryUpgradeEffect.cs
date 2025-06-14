using UnityEngine;

[CreateAssetMenu(fileName = "ParryUpgradeEffect", menuName = "Upgrades/ParryUpgradeEffect")]
public class ParryUpgradeEffect : UpgradeEffectBase
{
    public float radius;
    public float cooldown;

    public override void Apply(PlayerController player)
    {
        return;
    }

    public override void PerformAbility(PlayerController player)
    {
        player.Parry(radius, cooldown);
        Debug.Log("Parry performed");
    }
}
