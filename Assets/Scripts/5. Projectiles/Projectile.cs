using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    public float projectileSpeed;
    public int maxBounces;
    //[SerializeField] private float maxLifetime = 10f;

    private Vector3 direction;
    private int bounceCount = 0;

    public Action OnProjectileDestroyed; // <- добавлено
    
    private bool hasEnteredEnemyZone = false;

    private void Start()
    {
        direction = transform.forward;
        //Invoke(nameof(DestroySelf), maxLifetime);
    }

    private void Update()
    {
        float distance = projectileSpeed * Time.deltaTime;
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.1f, direction, out hit, distance))
        {
            if (hit.collider.CompareTag("PlayerWall"))
            {
                transform.position += direction * distance;
                return;
            }
            
            if (hit.collider.CompareTag("CenterWall"))
            {
                if (!hasEnteredEnemyZone)
                {
                    hasEnteredEnemyZone = true;
                    gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
                    transform.position = hit.point + direction * 0.01f;
                    return;
                }
            }
            
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(1);
            }
            
            HandleBounce(hit.normal, hit.point);
        }
        else
        {
            transform.position += direction * distance;
        }
    }
    
    private void HandleBounce(Vector3 normal, Vector3 hitPoint)
    {
        direction = ReflectInXZ(direction, normal);
        bounceCount++;
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

    private void DestroySelf()
    {
        OnProjectileDestroyed?.Invoke();
        Destroy(gameObject);
    }
}