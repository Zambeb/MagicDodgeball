using UnityEngine;

[CreateAssetMenu(fileName = "DashUpgrade", menuName = "Upgrades/DashUpgrade")]
public class DashUpgradeEffect : UpgradeEffectBase
{
    public float dashDistance = 5;
    public float dashDuration = 0.3f;
    public override void Apply(PlayerController player)
    {
        return;
    }

    public override void PerformAbility(PlayerController player)
    {
        player.Dash(dashDistance, dashDuration);
        Debug.Log("Dash " + dashDistance + " meters performed!");
    }
}