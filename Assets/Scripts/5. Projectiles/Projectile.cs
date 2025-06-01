using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    
    public int playerIndex;
    public float projectileSpeed;
    public int maxBounces;
    public float accelerationAfterBounce;
    //[SerializeField] private float maxLifetime = 10f;
    public bool canStun;
    public float stunDuration;
    public bool miniBall = false;

    public Vector3 direction;
    private int bounceCount = 0;

    public Action OnProjectileDestroyed; 
    
    private bool hasEnteredEnemyZone = false;
    private float sphereCastRadius;
    
    public GameObject burnedAreaPrefab; 
    public PlayerController ownerPlayer;
    
    [Header("Mini Projectiles")]
    public GameObject miniProjectilePrefab;
    public int explosionProjectileCount = 6; 
    public float explosionProjectileSpeed = 5f; 
    public float explosionSpreadAngle = 360f; 

    private void Start()
    {
        transform.localScale *= ownerPlayer.stats.ballSizeMultiplier;
        if (direction == Vector3.zero)
        {
            direction = transform.forward;
        }
        //Invoke(nameof(DestroySelf), maxLifetime);
        SphereCollider sc = GetComponent<SphereCollider>();
        if (sc != null)
        {
            sphereCastRadius = sc.radius * Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            sphereCastRadius = transform.localScale.x / 2f;
        }
    }
    
    public void Initialize(Vector3 initDirection)
    {
        direction = initDirection;
    }

    private void FixedUpdate()
    {
        float totalDistance = projectileSpeed * Time.deltaTime;
        int steps = Mathf.CeilToInt(totalDistance / sphereCastRadius);
        float stepDistance = totalDistance / steps;

        for (int i = 0; i < steps; i++)
        {
            if (DoMovementStep(stepDistance))
            {
                break;
            }
        }
    }
    private bool DoMovementStep(float distance)
{
    if (!Physics.SphereCast(transform.position, sphereCastRadius, direction, out RaycastHit hit, distance, collisionMask))
    {
        transform.position += direction * distance;
        return false;
    }

    // Cash tag
    string hitTag = hit.collider.tag;

    // 1. PlayerWall - let the ball through
    if (hitTag == "PlayerWall")
    {
        transform.position += direction * distance;
        return true;
    }

    // 2. CenterWall - change layer and let the ball through
    if (!hasEnteredEnemyZone && !miniBall && hitTag == "CenterWall")
    {
        hasEnteredEnemyZone = true;
        gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
        transform.position += direction * distance;
        return false;
    }

    // 3. Shield
    if (hitTag == "Shield")
    {
        var shieldOwner = hit.collider.GetComponentInParent<PlayerController>();
        if (shieldOwner != null)
        {
            if (shieldOwner.playerIndex != playerIndex)
            {
                HandleBounce(hit.normal, hit.point);
                return true;
            }
            
            transform.position = direction * distance;
            return false;
        }
    }

    // 4. IDamageable
    var damageable = hit.collider.GetComponent<IDamageable>();
    if (damageable != null)
    {
        var hitPlayer = hit.collider.GetComponent<PlayerController>();
        bool isSelfHit = hitPlayer != null && hitPlayer.playerIndex == playerIndex;

        if (canStun)
        {
            damageable.Stun(stunDuration);
        }

        if (!isSelfHit || (isSelfHit && hitPlayer.stats.canSelfHarm))
        {
            damageable.TakeDamage();
        }
    }

    // 5. Bounce
    HandleBounce(hit.normal, hit.point);
    return true;
}

    private void HandleBounce(Vector3 normal, Vector3 hitPoint)
    {
        bounceCount++;
        if (bounceCount >= maxBounces)
        {
            DestroySelf();
            return;
        }
        
        direction = ReflectInXZ(direction, normal);
        projectileSpeed *= accelerationAfterBounce;
        transform.position = hitPoint + normal * sphereCastRadius + direction * 0.01f;
    }
    
    private Vector3 ReflectInXZ(Vector3 incoming, Vector3 normal)
    {
        Vector3 reflected = Vector3.Reflect(incoming, normal);
        reflected.y = 0f;
        return reflected.normalized;
    }

    public void DestroySelf()
    {
        if (ownerPlayer != null && !miniBall)
        {
            if (ownerPlayer.stats.canBurnArea)
            {
                Vector3 spawnPosition = transform.position - direction.normalized * ownerPlayer.stats.ballSizeMultiplier;
                GameObject burnedArea = Instantiate(burnedAreaPrefab, spawnPosition, Quaternion.identity);
                burnedArea.transform.localScale *= ownerPlayer.stats.ballSizeMultiplier;
            }

            if (ownerPlayer.stats.canExplodeBalls && miniProjectilePrefab != null)
            {
                for (int i = 0; i < explosionProjectileCount; i++)
                {
                    float angle = (i / (float)explosionProjectileCount) * 360f;
                    Vector3 dir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

                    GameObject miniProjObj = Instantiate(miniProjectilePrefab, transform.position, Quaternion.identity);
                    miniProjObj.transform.localScale *= (0.5f * ownerPlayer.stats.ballSizeMultiplier);
                    Projectile miniProj = miniProjObj.GetComponent<Projectile>();

                    if (miniProj != null)
                    {
                        miniProj.Initialize(dir);
                        miniProj.playerIndex = playerIndex;
                        miniProj.ownerPlayer = ownerPlayer;
                        miniProj.projectileSpeed = explosionProjectileSpeed;
                        miniProj.maxBounces = 1;
                        miniProj.direction = dir;
                        miniProj.canStun = false;
                        miniProj.miniBall = true;
                    }
                }
            }
        }
        OnProjectileDestroyed?.Invoke();
        Destroy(gameObject);
    }
}