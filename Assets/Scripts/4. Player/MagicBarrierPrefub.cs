using UnityEngine;

public class MagicBarrierPrefab : MonoBehaviour, IDamageable
{
    public int wallHP;
    private MagicBarrier barrierParent;

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