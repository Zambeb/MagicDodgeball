using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public int maxHealth;
    public int currentHealth;
    public float moveSpeed;
    public float projectileSpeed;
    public int maxBounces;
    public int maxProjectiles;


    public PlayerStats()
    {
    }

    public PlayerStats(PlayerStats other)
    {
        maxHealth = other.maxHealth;
        moveSpeed = other.moveSpeed;
        maxBounces = other.maxBounces;
        projectileSpeed = other.projectileSpeed;
        currentHealth = other.currentHealth;
        maxProjectiles = other.maxProjectiles;
    }
    
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}

