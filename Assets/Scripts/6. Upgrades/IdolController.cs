using System;
using System.Collections;
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

    public Animator animator;

    public GameObject instantiateVFX;
    public GameObject explodeVFX;
    
    public void Initialize(TrashIdolEffect parent)
    {
        idolParent = parent;

        idolHP = parent.idolHP;
        bounces = parent.idolBallBounces;
        ballSpeed = parent.idolBallSpeed;
        shootInterval = parent.idolShootInterval;
        
        gun.ownerPlayer = ownerPlayer;
        opponentPlayer = ownerPlayer.opponent;
        shootTimer = shootInterval;
        
        Quaternion randomRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
        GameObject effect = Instantiate(instantiateVFX, transform.position, randomRotation);
        Destroy(effect, 2);
        
        StartCoroutine(SpawnRise());
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
            if (animator != null)
            {
                Debug.Log("Set Attack Trigger");
                animator.SetTrigger("Attack");
            }
            gun.Shoot(ownerPlayer.playerIndex, bounces, ballSpeed); 
            
            shootTimer = shootInterval;
        }
    }
    
    private IEnumerator SpawnRise()
    {
        float duration = 0.5f;
        float elapsed = 0f;

        Vector3 startPos = transform.position + new Vector3(0, -2f, 0);
        Vector3 endPos = transform.position;

        transform.position = startPos;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Можно добавить плавность с помощью SmoothStep
            t = Mathf.SmoothStep(0, 1, t);

            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;
    }
    
    public void Stun(float duration)
    {
        return;
    }

    public void TakeDamage()
    {
        idolHP -= 1;
        Debug.Log("Idol HP = " + idolHP);
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
        GameObject effect = Instantiate(explodeVFX, transform.position, Quaternion.identity);
        effect.transform.localScale *= 0.5f;
        SoundManager.Instance.PlaySFX("Explosion", gameObject.transform.position);
        Destroy(effect, 2);
        
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
