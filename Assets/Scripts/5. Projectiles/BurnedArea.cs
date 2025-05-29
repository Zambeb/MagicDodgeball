using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnedArea : MonoBehaviour
{
    public float duration = 5f;
    public float damageInterval = 1f;

    private HashSet<IDamageable> targetsInside = new HashSet<IDamageable>();

    private void Start()
    {
        StartCoroutine(DestroyAfterTime());
        StartCoroutine(DamageCoroutine());
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    private IEnumerator DamageCoroutine()
    {
        while (true)
        {
            foreach (var target in targetsInside)
            {
                if (target != null)
                {
                    target.TakeDamage();
                }
            }
            yield return new WaitForSeconds(damageInterval);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            targetsInside.Add(damageable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            targetsInside.Remove(damageable);
        }
    }
}