using UnityEngine;

[CreateAssetMenu(fileName = "ChargingShot", menuName = "Upgrades/ChargingShot")]
public class ChargingShotEffect : UpgradeEffectBase
{
    public float maxChargeTime;
    public float chargeSpeedIncreasePerSecond;
    
    public override void Apply(PlayerController player)
    {
        player.stats.canCharge = true;
        player.stats.maxChargeTime = maxChargeTime;
        player.stats.chargeSpeedIncreasePerSecond = chargeSpeedIncreasePerSecond;
    }

    public override void PerformAbility(PlayerController player)
    {
        return;
    }
}
