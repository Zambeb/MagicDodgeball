using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    public float projectileSpeed;
    public int maxBounces;
    public float accelerationAfterBounce;
    //[SerializeField] private float maxLifetime = 10f;
    public bool canStun;
    public float stunDuration;

    private Vector3 direction;
    private int bounceCount = 0;

    public Action OnProjectileDestroyed; 
    
    private bool hasEnteredEnemyZone = false;
    private float sphereCastRadius;

    private void Start()
    {
        direction = transform.forward;
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
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, sphereCastRadius, direction, out hit, distance))
        {
            if (hit.collider.CompareTag("PlayerWall"))
            {
                transform.position += direction * distance;
                return true;
            }

            if (hit.collider.CompareTag("CenterWall"))
            {
                if (!hasEnteredEnemyZone)
                {
                    hasEnteredEnemyZone = true;
                    gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
                    transform.position = hit.point + direction * 0.01f;
                    return true;
                }
            }

            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                if (canStun)
                {
                    damageable.Stun(stunDuration);
                }
                damageable.TakeDamage();
            }

            HandleBounce(hit.normal, hit.point);
            return true;
        }
        else
        {
            transform.position += direction * distance;
            return false;
        }
    }

    private void HandleBounce(Vector3 normal, Vector3 hitPoint)
    {
        direction = ReflectInXZ(direction, normal);
        bounceCount++;
        projectileSpeed *= accelerationAfterBounce;
        if (bounceCount > maxBounces)
        {
            DestroySelf();
            return;
        }
        transform.position = hitPoint + direction * 0.01f;
    }
    
    private Vector3 ReflectInXZ(Vector3 incoming, Vector3 normal)
    {
        Vector3 reflected = Vector3.Reflect(incoming, normal);
        reflected.y = 0f;
        return reflected.normalized;
    }

    public void DestroySelf()
    {
        OnProjectileDestroyed?.Invoke();
        Destroy(gameObject);
    }
}