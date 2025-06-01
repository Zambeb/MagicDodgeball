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
    }
}

