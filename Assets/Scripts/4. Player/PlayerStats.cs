using UnityEngine;

[System.Serializable]
public struct PlayerStats
{
    public int currentHealth;
    public float moveSpeed;
    public float projectileSpeed;
    public int maxBounces;
    public int maxProjectiles;
    public float accelerationAfterBounce;
    public bool canStun;
    public float stunDuration;
    public bool canSelfHarm;
    public bool canBurnArea;
    public float ballSizeMultiplier;
    public bool canExplodeBalls;
    public bool amigaBall;
    public float immunityAfterHit;
    public bool canAbsorbBalls;
    public bool leavesTrail;
    public float trailDuration;
    public float slowAmount;
    public bool canCharge;
    public float maxChargeTime;
    public float chargeSpeedIncreasePerSecond;
    public int miniBallsQuantity;
    public float miniBallsSizeMultiplier;

    /*
    public PlayerStats()
    {
    }
    */

    public PlayerStats(PlayerStats other)
    {
        moveSpeed = other.moveSpeed;
        maxBounces = other.maxBounces;
        projectileSpeed = other.projectileSpeed;
        currentHealth = other.currentHealth;
        maxProjectiles = other.maxProjectiles;
        accelerationAfterBounce = other.accelerationAfterBounce;
        canStun = other.canStun;
        stunDuration = other.stunDuration;
        canSelfHarm = other.canSelfHarm;
        canBurnArea = other.canBurnArea;
        ballSizeMultiplier = other.ballSizeMultiplier;
        canExplodeBalls = other.canExplodeBalls;
        amigaBall = other.amigaBall;
        immunityAfterHit = other.immunityAfterHit;
        canAbsorbBalls = other.canAbsorbBalls;
        leavesTrail = other.leavesTrail;
        trailDuration = other.trailDuration;
        slowAmount = other.slowAmount;
        canCharge = other.canCharge;
        maxChargeTime = other.maxChargeTime;
        chargeSpeedIncreasePerSecond = other.chargeSpeedIncreasePerSecond;
        miniBallsQuantity = other.miniBallsQuantity;
        miniBallsSizeMultiplier = other.miniBallsSizeMultiplier;
    }
}

