using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public int maxHealth = 3;
    public int currentHealth;
    public float moveSpeed = 5f;
    public float projectileSpeed = 10f;
    public int maxBounces = 3;

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}

