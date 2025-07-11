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
    //public Material[] projectileMaterial;
    //public Material amigaBallMaterial;
    private GameObject projectileCollector;

    public static PlayerGun Instance;

    private PlayerController playerController;

    public List<GameObject> activeProjectiles = new List<GameObject>();
    
    public List<MonoBehaviour> bruh = new List<MonoBehaviour>();

    private void Awake()
    {
        Instance = this;
        playerController = GetComponent<PlayerController>();
        projectileCollector = GameObject.Find("ProjectileCollector");
    }

    public void Shoot(int index, int bounces, float speed, float acceleration, bool canStun, float stunDuration, bool leavesTrail)
    {
        RoundManager.Instance.projectileCount++;
        GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation, projectileCollector.transform);
        
        Projectile projectileProj = projectile.GetComponent<Projectile>();
        projectileProj.ownerPlayer = playerController;
        projectileProj.maxBounces = bounces;
        projectileProj.projectileSpeed = speed;
        projectileProj.accelerationAfterBounce = acceleration;
        projectileProj.canStun = canStun;
        projectileProj.stunDuration = stunDuration;
        projectileProj.playerIndex = index;
        projectileProj.projectileCount = RoundManager.Instance.projectileCount;
        activeProjectiles.Add(projectile);
        Renderer rend = projectile.GetComponent<Renderer>();

        projectileProj.ballsVisuals[index].SetActive(true);

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.OnProjectileDestroyed = () =>
        {
            activeProjectiles.Remove(projectile);
            
            int usedBalls = activeProjectiles.Count;
            int notUsedBalls = playerController.stats.maxProjectiles - activeProjectiles.Count;

            UIManager.Instance.UpdateBallsDisplay(index, notUsedBalls, usedBalls);
        };
        if (leavesTrail)
        {
            projectileProj.leavesTrail = true;
            projectileProj.trailDuration = playerController.stats.trailDuration;
            projectileProj.slowAmount = playerController.stats.slowAmount;
        }
    }
}