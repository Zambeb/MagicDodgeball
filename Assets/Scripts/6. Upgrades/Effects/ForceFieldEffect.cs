using UnityEngine;

[CreateAssetMenu(fileName = "ForceFieldUpgrade", menuName = "Upgrades/ForceFieldDupgrade")]

public class ForceFieldEffect : UpgradeEffectBase
{
    public float forceFieldDuration = 2;
    public float speedMultiplier = 2;

    public override void Apply(PlayerController player)
    {
        return;
    }

    public override void PerformAbility(PlayerController player)
    {
        player.ForceField(forceFieldDuration, speedMultiplier);
        Debug.Log("Force field performed!");
    }
}
