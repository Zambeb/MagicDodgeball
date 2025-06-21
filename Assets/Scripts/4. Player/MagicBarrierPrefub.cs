using UnityEngine;

public class MagicBarrierPrefab : MonoBehaviour, IDamageable
{
    public int wallHP;
    private MagicBarrier barrierParent;
    public GameObject destroyEffectPrefab;

    public void Initialize(MagicBarrier parent)
    {
        barrierParent = parent;
        wallHP = parent.barrierHP;
    }

    public void Stun(float duration)
    {
        return;
    }

    public void TakeDamage()
    {
        wallHP -= 1;
        CheckIfDead();
    }

    public void CheckIfDead()
    {
        if (wallHP <= 0)
        {
            DestroySelf();
        }
    }

    public void DestroySelf()
    {
        if (destroyEffectPrefab != null)
        {
            Quaternion randomRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            Vector3 spawnPos = transform.position;
            spawnPos.y = 1f;
            GameObject effect = Instantiate(destroyEffectPrefab, spawnPos, randomRotation);
            Destroy(effect, 2);
        }
        
        if (barrierParent != null)
        {
            barrierParent.RemoveBarrier(this);
        }
        
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (barrierParent != null)
        {
            barrierParent.RemoveBarrier(this);
        }
    }
}