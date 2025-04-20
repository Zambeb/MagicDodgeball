using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float projectileSpeed = 10f;
    //[SerializeField] private float maxLifetime = 10f;
    [SerializeField] private int maxBounces = 5;

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
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(1);
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
            
            if (hit.collider.CompareTag("Projectile"))
            {
                direction = Vector3.Reflect(direction, hit.normal);
                bounceCount++;
                if (bounceCount >= maxBounces) DestroySelf();
                transform.position = hit.point + direction * 0.01f;
                return;
            }

            direction = Vector3.Reflect(direction, hit.normal);
            bounceCount++;
            if (bounceCount >= maxBounces) DestroySelf();
            transform.position = hit.point + direction * 0.01f;
        }
        else
        {
            transform.position += direction * distance;
        }
    }
    

    private void DestroySelf()
    {
        OnProjectileDestroyed?.Invoke();
        Destroy(gameObject);
    }
}