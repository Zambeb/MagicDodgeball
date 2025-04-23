using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public int maxHealth;
    public int currentHealth;
    public float moveSpeed;
    public float projectileSpeed;
    public int maxBounces;

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}

