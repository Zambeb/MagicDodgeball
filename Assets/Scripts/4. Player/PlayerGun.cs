using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float firingSpeed;
    //[SerializeField] private int maxProjectiles = 3;
    [SerializeField] private Material[] projectileMaterial;

    public static PlayerGun Instance;

    private PlayerController playerController;

    private List<GameObject> activeProjectiles = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
        playerController = GetComponent<PlayerController>();
    }

    public void Shoot(int index, int bounces, float speed)
    {
        int maxProjectiles = playerController.stats.maxProjectiles;

        if (activeProjectiles.Count >= maxProjectiles)
            return;

        GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation);
        projectile.GetComponent<Projectile>().maxBounces = bounces;
        projectile.GetComponent<Projectile>().projectileSpeed = speed;
        activeProjectiles.Add(projectile);
        Renderer rend = projectile.GetComponent<Renderer>();
        rend.material = projectileMaterial[index];
        
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.OnProjectileDestroyed = () => { activeProjectiles.Remove(projectile); };
    }
}