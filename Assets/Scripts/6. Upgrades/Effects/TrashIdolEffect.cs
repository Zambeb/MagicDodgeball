using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrashIdolEffect", menuName = "Upgrades/Active/TrashIdolEffect")]
public class TrashIdolEffect : UpgradeEffectBase
{

    public float cooldown = 10f;
    public GameObject idolPrefab;
    public int maxIdols = 1;
    public int idolHP = 3;
    public float idolDistance = 2f;
    public float idolShootInterval = 2f;
    public float idolBallSpeed = 10f;
    public int idolBallBounces = 1;

    private List<IdolController> activeIdols = new List<IdolController>();
    private bool isOnCooldown = false;
    
    public override void Apply(PlayerController player)
    {
        return;
    }

    public override void PerformAbility(PlayerController player)
    {
        if (player.IsActiveOnCooldown || isOnCooldown || idolPrefab == null) return;
        
        player.SetActiveCooldown(cooldown);

        activeIdols.RemoveAll(idol => idol == null);

        Transform playerTransform = player.transform;
        Vector3 spawnPostition = playerTransform.position + player.transform.forward * idolDistance;

        GameObject newIdolObj =
            Instantiate(idolPrefab, spawnPostition, Quaternion.LookRotation(playerTransform.forward));

        IdolController newIdol = newIdolObj.GetComponent<IdolController>();

        newIdol.ownerPlayer = player;
        newIdol.opponentPlayer = player.opponent;
        
        newIdol.Initialize(this);
        
        activeIdols.Add(newIdol);

        if (activeIdols.Count > maxIdols && activeIdols[0] != null)
        {
            activeIdols[0].DestroySelf();
        }
    }

    public void RemoveIdol(IdolController idol)
    {
        activeIdols.Remove(idol);
    }

    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }

    public void RemoveAllIdols()
    {
        var idolsToRemove = new List<IdolController>(activeIdols);

        foreach (var idol in idolsToRemove)
        {
            if (idol != null)
            {
                idol.DestroySelf();
            }
        }
        activeIdols.Clear();
    }
}
