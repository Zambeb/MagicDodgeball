using System;
using UnityEngine;

public class IdolController : MonoBehaviour, IDamageable
{
    public int idolHP;
    public PlayerController ownerPlayer;
    public PlayerController opponentPlayer;
    public IdolGun gun;
    private TrashIdolEffect idolParent;

    public float shootInterval;
    public int bounces;
    public float ballSpeed;
    private float shootTimer;
    
    public void Initialize(TrashIdolEffect parent)
    {
        idolParent = parent;

        idolHP = parent.idolHP;
        bounces = parent.idolBallBounces;
        ballSpeed = parent.idolBallSpeed;
        shootInterval = parent.idolShootInterval;
        
        gun.ownerPlayer = ownerPlayer;
        opponentPlayer = ownerPlayer.opponent;
        shootTimer = 0;
    }
    
    void Update()
    {
        if (opponentPlayer != null)
        {
            Vector3 targetPos = opponentPlayer.transform.position;
            Vector3 direction = targetPos - transform.position;
            direction.y = 0;
            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }
        }
        
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f && RoundManager.Instance.roundActive)
        {
            gun.Shoot(ownerPlayer.playerIndex, bounces, ballSpeed); 
            shootTimer = shootInterval;
        }
    }
    
    public void Stun(float duration)
    {
        return;
    }

    public void TakeDamage()
    {
        idolHP -= 1;
        CheckIfDead();
    }

    public void CheckIfDead()
    {
        if (idolHP <= 0)
        {
            DestroySelf();
        }
    }
    
    public void DestroySelf()
    {
        if (idolParent != null)
        {
            idolParent.RemoveIdol(this);
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (idolParent != null)
        {
            idolParent.RemoveIdol(this);
        }
    }
}
