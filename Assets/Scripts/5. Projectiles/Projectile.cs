using UnityEngine;
using System;
using System.Collections;
using Unity.VisualScripting;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;

    public int projectileCount;
    
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

    [Header("Visuals")] 
    public GameObject[] ballsVisuals;
    
    [Header("Mini Projectiles")] 
    public bool isMiniBall = false;
    public GameObject miniProjectilePrefab;
    public int explosionProjectileCount;
    public float explosionProjectileSizeMultiplier;
    public float explosionProjectileSpeed = 5f; 
    public float explosionSpreadAngle = 360f;

    [Header("Trail")] 
    public bool leavesTrail;
    [SerializeField] private GameObject trailObject;
    [SerializeField] private float trailSpacing = 0.5f;
    [SerializeField] private GameObject trailPrefab;
    private Vector3 lastTrailSpawnPos;
    public float trailDuration;
    public float slowAmount;
    
    [Header("Visual Effects")]
    public GameObject bounceEffectPrefab; 
    public GameObject destroyEffectPrefab;
    public GameObject explodeEffectPrefab;
    public GameObject mergeEffectPrefab;
    public float effectLifetime = 2f;

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
            float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
            sphereCastRadius = sc.radius * maxScale;
            
            sphereCastRadius *= 1.1f;
        }
        else
        {
            float fallbackScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
            sphereCastRadius = fallbackScale / 2f;
            sphereCastRadius *= 1.1f; 
        }
        sphereCastRadius = Mathf.Max(sphereCastRadius, 0.5f);

        if (leavesTrail)
        {
            trailObject.SetActive(true);
            trailObject.GetComponent<TrailRenderer>().time = ownerPlayer.stats.trailDuration;
            lastTrailSpawnPos = transform.position;
        }
    }
    
    public void Initialize(Vector3 initDirection)
    {
        direction = initDirection;
    }

    private void FixedUpdate()
    {
        float maxDistancePerFrame = sphereCastRadius * 10f;
        float totalDistance = Mathf.Min(projectileSpeed * Time.deltaTime, maxDistancePerFrame);
        float stepSize = 0.01f;
        int steps = Mathf.CeilToInt(totalDistance / stepSize);
        float stepDistance = totalDistance / steps;

        for (int i = 0; i < steps; i++)
        {
            if (DoMovementStep(stepDistance))
            {
                break;
            }
        }

        if (leavesTrail)
        {
            float dist = Vector3.Distance(transform.position, lastTrailSpawnPos);
            if (dist >= trailSpacing)
            {
                SpawnTrail(trailDuration);
                lastTrailSpawnPos = transform.position;
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sphereCastRadius);
    }
    
    void SpawnTrail(float dur)
    {
        GameObject trail = Instantiate(trailPrefab, transform.position, Quaternion.identity);
        trail.transform.localScale *= ownerPlayer.stats.ballSizeMultiplier/2;
        TrailSlowZone trailSlow = trail.GetComponent<TrailSlowZone>();
        if (trailSlow != null)
        {
            trailSlow.slowAmount = slowAmount;
        }
        Destroy(trail, dur);
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
    if (hitTag == "PlayerWall" || hitTag == "Trail" )
    {
        transform.position += direction * distance;
        return false;
    }

    // 2. CenterWall - change layer and let the ball through
    if (hitTag == "CenterWall")
    {
        if (!hasEnteredEnemyZone && !miniBall)
        {
            hasEnteredEnemyZone = true;
            gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
            transform.position += direction * distance;
            return false;
        }
        else
        {
            HandleBounce(hit.normal, hit.point);
            return true;
        }
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
            
            transform.position += direction * distance;
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
    // 5. Projectile
    if (hitTag == "Projectile")
    {
        var proj = hit.collider.GetComponent<Projectile>();
        if (proj != null)
        {
            bool iCanAbsorb = ownerPlayer.stats.canAbsorbBalls && !isMiniBall;
            bool otherCanAbsorb = proj.ownerPlayer != null && proj.ownerPlayer.stats.canAbsorbBalls && !proj.isMiniBall;

            if (iCanAbsorb || otherCanAbsorb)
            {
                bool iShouldAbsorb;
                
                if (iCanAbsorb != otherCanAbsorb)
                {
                    iShouldAbsorb = iCanAbsorb;
                }
                else
                {
                    if (projectileCount != proj.projectileCount)
                    {
                        iShouldAbsorb = projectileCount > proj.projectileCount;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (iShouldAbsorb)
                {
                    AbsorbBall(proj);
                    transform.position += direction * distance;
                    return true;
                }
            }
        }
    }

    // 6. Bounce
    HandleBounce(hit.normal, hit.point);
    return true;
}

    private void HandleBounce(Vector3 normal, Vector3 hitPoint)
    {
        if (bounceEffectPrefab != null)
        {
            Quaternion effectRotation = Quaternion.LookRotation(normal);
            GameObject effect = Instantiate(bounceEffectPrefab, hitPoint, effectRotation);
            Destroy(effect, effectLifetime);
        }
        
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

    public void Redirect(Vector3 newDirection)
    {
        Debug.Log("Redirection");
        direction = newDirection.normalized;
        transform.forward = direction;
        hasEnteredEnemyZone = false;
        //bounceCount++;
    }

    private void AbsorbBall(Projectile absorbed)
    {
        if (absorbed == null || !absorbed.gameObject.activeSelf || this == null) return;

        int remainingBounces = absorbed.maxBounces - absorbed.bounceCount;
        maxBounces += Mathf.Max(0, remainingBounces); 
        
        Vector3 scaleIncrease = absorbed.transform.localScale * 0.5f;
        transform.localScale += scaleIncrease;
        
        SphereCollider sc = GetComponent<SphereCollider>();
        float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

        if (sc != null)
        {
            sphereCastRadius = sc.radius * maxScale * 1.1f;
        }
        else
        {
            sphereCastRadius = maxScale / 2f;
        }
        sphereCastRadius = Mathf.Max(sphereCastRadius, 0.5f);
        
        StartCoroutine(PulseScale());

        if (mergeEffectPrefab != null)
        {
            GameObject effect = Instantiate(mergeEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.localScale = this.GameObject().transform.localScale;
            Destroy(effect, effectLifetime);
        }
        
        absorbed.DestroySelf();
        Debug.Log($"Absorbing: scale increased for {scaleIncrease}");
    }
    
    private IEnumerator PulseScale()
    {
        Debug.Log("Пульсирую");
        Vector3 baseScale = transform.localScale;
        
        Vector3 targetScale1 = baseScale * 1.5f;
        Vector3 targetScale11 = baseScale * 0.5f;
        float duration1 = 0.15f; 
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration1;
            transform.localScale = Vector3.Lerp(baseScale, targetScale1, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration1;
            transform.localScale = Vector3.Lerp(targetScale1, targetScale11, t);
            yield return null;
        }
        
        Vector3 targetScale2 = baseScale * 1.25f;
        Vector3 targetScale22 = baseScale * 0.75f;
        float duration2 = 0.12f;
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration2;
            transform.localScale = Vector3.Lerp(baseScale, targetScale2, t);
            yield return null;
        }
        
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration2;
            transform.localScale = Vector3.Lerp(baseScale, targetScale22, t);
            yield return null;
        }
        
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration2;
            transform.localScale = Vector3.Lerp(targetScale2, baseScale, t);
            yield return null;
        }
    }

    public void DestroySelf()
    {
        if (destroyEffectPrefab != null && !ownerPlayer.stats.canBurnArea && !ownerPlayer.stats.canAbsorbBalls)
        {
            Quaternion randomRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            GameObject effect = Instantiate(destroyEffectPrefab, transform.position, randomRotation);
            Destroy(effect, effectLifetime);
        }
        
        if (ownerPlayer != null && !miniBall)
        {
            if (ownerPlayer.stats.canBurnArea)
            {
                Vector3 spawnPosition = transform.position - direction.normalized * ownerPlayer.stats.ballSizeMultiplier;
                spawnPosition.y = 0;
                GameObject burnedArea = Instantiate(burnedAreaPrefab, spawnPosition, Quaternion.identity);
                float adjustedScale = 1f + (ownerPlayer.stats.ballSizeMultiplier - 1f) * 0.5f;
                burnedArea.transform.localScale *= adjustedScale;
                SoundManager.Instance.PlaySFX("Explosion", spawnPosition);
            }

            if (ownerPlayer.stats.canExplodeBalls && miniProjectilePrefab != null)
            {
                Quaternion randomRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
                GameObject effect = Instantiate(explodeEffectPrefab, transform.position, randomRotation);
                SoundManager.Instance.PlaySFX("Explosion", effect.transform.position);
                Destroy(effect, effectLifetime);

                explosionProjectileCount = ownerPlayer.stats.miniBallsQuantity;
                explosionProjectileSizeMultiplier = ownerPlayer.stats.miniBallsSizeMultiplier;
                
                for (int i = 0; i < explosionProjectileCount; i++)
                {
                    float angle = (i / (float)explosionProjectileCount) * 360f;
                    Vector3 dir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

                    GameObject miniProjObj = Instantiate(miniProjectilePrefab, transform.position, Quaternion.identity);
                    miniProjObj.transform.localScale = Vector3.one * explosionProjectileSizeMultiplier;
                    Projectile miniProj = miniProjObj.GetComponent<Projectile>();

                    if (miniProj != null)
                    {
                        miniProj.Initialize(dir);
                        miniProj.isMiniBall = true;
                        miniProj.playerIndex = playerIndex;
                        miniProj.ownerPlayer = ownerPlayer;
                        miniProj.projectileSpeed = explosionProjectileSpeed;
                        miniProj.maxBounces = 1;
                        miniProj.direction = dir;
                        miniProj.canStun = false;
                        miniProj.miniBall = true;
                        miniProj.ballsVisuals[playerIndex].SetActive(true);
                    }
                }
            }
        }

        if (leavesTrail && trailObject != null)
        {
            trailObject.transform.SetParent(null);
            var trail = trailObject.GetComponent<TrailRenderer>();
            if (trail != null)
            {
                trail.autodestruct = true;
            }
        }
        
        OnProjectileDestroyed?.Invoke();
        Destroy(gameObject);
    }
}