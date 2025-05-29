using UnityEngine;

[System.Serializable]
public class PlayerStats
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
    }
}

