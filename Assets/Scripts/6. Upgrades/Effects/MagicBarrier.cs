using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MagicBarrier", menuName = "Upgrades/Active/MagicBarrier")]

public class MagicBarrier : UpgradeEffectBase
{
    public float cooldown = 4f;
    public GameObject barrierPrefab;
    public int maxBarriers = 2;
    public int barrierHP = 2;

    public float wallDistance = 2f;
    private List<MagicBarrierPrefab> activeBarriers = new List<MagicBarrierPrefab>();
    
    public override void Apply(PlayerController player)
    {
        return;
    }

    public override void PerformAbility(PlayerController player)
    {
        if (barrierPrefab == null) return;
        
        activeBarriers.RemoveAll(barrier => barrier == null);

        Transform playerTransform = player.transform;
        Vector3 spawnPosition = playerTransform.position + playerTransform.forward * wallDistance;

        GameObject newWallObj = Instantiate(barrierPrefab, spawnPosition, Quaternion.LookRotation(playerTransform.forward));
        MagicBarrierPrefab newWall = newWallObj.GetComponent<MagicBarrierPrefab>();
        newWall.Initialize(this); 
        
        activeBarriers.Add(newWall);
        
        if (activeBarriers.Count > maxBarriers && activeBarriers[0] != null)
        {
            activeBarriers[0].DestroySelf();
        }
    }
    
    public void RemoveBarrier(MagicBarrierPrefab barrier)
    {
        activeBarriers.Remove(barrier);
    }
}
