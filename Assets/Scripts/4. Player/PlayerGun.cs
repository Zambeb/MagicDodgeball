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
    private GameObject projectileCollector;

    public static PlayerGun Instance;

    private PlayerController playerController;

    private List<GameObject> activeProjectiles = new List<GameObject>();
    
    public List<MonoBehaviour> bruh = new List<MonoBehaviour>();

    private void Awake()
    {
        Instance = this;
        playerController = GetComponent<PlayerController>();
        projectileCollector = GameObject.Find("ProjectileCollector");
    }

    public void Shoot(int index, int bounces, float speed, float acceleration)
    {
        int maxProjectiles = playerController.stats.maxProjectiles;

        if (activeProjectiles.Count >= maxProjectiles)
            return;

        GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation, projectileCollector.transform);
        Projectile projectileProj = projectile.GetComponent<Projectile>();
        projectileProj.maxBounces = bounces;
        projectileProj.projectileSpeed = speed;
        projectileProj.accelerationAfterBounce = acceleration;
        activeProjectiles.Add(projectile);
        Renderer rend = projectile.GetComponent<Renderer>();
        rend.material = projectileMaterial[index];

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.OnProjectileDestroyed = () => { activeProjectiles.Remove(projectile); };
    }
}