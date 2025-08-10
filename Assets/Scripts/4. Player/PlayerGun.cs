using System;
using System.Collections;
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
        FireProjectile(index, bounces, speed, acceleration, canStun, stunDuration, leavesTrail, true);

        if (playerController.stats.doubleShot)
        {
            float roll = UnityEngine.Random.value;
            if (roll <= playerController.stats.doubleShotChance)
            {
                StartCoroutine(DoubleShotCoroutine(index, bounces, speed, acceleration, canStun, stunDuration, leavesTrail));
            }
        }
    }

    private void FireProjectile(int index, int bounces, float speed, float acceleration, bool canStun, float stunDuration, bool leavesTrail, bool countAsUsed)
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

        if (countAsUsed)
        {
            activeProjectiles.Add(projectile);
        }

        projectileProj.ballsVisuals[index].SetActive(true);

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        
        projectileScript.OnProjectileDestroyed = () =>
        {
            if (countAsUsed)
            {
                activeProjectiles.Remove(projectile);
            
                int usedBalls = activeProjectiles.Count;
                int notUsedBalls = playerController.stats.maxProjectiles - activeProjectiles.Count;

                UIManager.Instance.UpdateBallsDisplay(index, notUsedBalls, usedBalls);
            }
        };
        
        if (leavesTrail)
        {
            projectileProj.leavesTrail = true;
            projectileProj.trailDuration = playerController.stats.trailDuration;
            projectileProj.slowAmount = playerController.stats.slowAmount;
        }
    }
    
    private IEnumerator DoubleShotCoroutine(int index, int bounces, float speed, float acceleration, bool canStun, float stunDuration, bool leavesTrail)
    {
        yield return new WaitForSeconds(playerController.stats.doubleShotInterval);
        FireProjectile(index, bounces, speed, acceleration, canStun, stunDuration, leavesTrail, false);
    }
}