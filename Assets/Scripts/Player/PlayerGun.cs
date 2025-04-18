using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float firingSpeed;
    [SerializeField] private int maxProjectiles = 3;

    public static PlayerGun Instance;

    private List<GameObject> activeProjectiles = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    public void Shoot()
    {
        if (activeProjectiles.Count >= maxProjectiles)
            return;

        GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation);
        activeProjectiles.Add(projectile);
        
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.OnProjectileDestroyed = () => { activeProjectiles.Remove(projectile); };
    }
}