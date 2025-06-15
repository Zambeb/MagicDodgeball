using System;
using System.Collections.Generic;
using UnityEngine;

public class TrailSlowZone : MonoBehaviour
{
    public float slowAmount;
    private HashSet<PlayerController> playersInZone = new HashSet<PlayerController>();

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null && playersInZone.Contains(player))
        {
            player.ApplySlow(slowAmount);
            playersInZone.Add(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null && playersInZone.Contains(player))
        {
            player.RemoveSlow();
            playersInZone.Remove(player);
        }
    }

    private void OnDestroy()
    {
        foreach (var player in playersInZone)
        {
            player.RemoveSlow();
        }

        playersInZone.Clear();
    }
}