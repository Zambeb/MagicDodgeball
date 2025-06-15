using System.Collections.Generic;
using UnityEngine;

public class TrailSlowZone : MonoBehaviour
{
    public float slowAmount;

    private readonly HashSet<PlayerController> playersInZone = new HashSet<PlayerController>();

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null && playersInZone.Add(player))
        {
            player.RegisterSlowZone(this, slowAmount);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null && playersInZone.Remove(player))
        {
            player.UnregisterSlowZone(this);
        }
    }

    private void OnDestroy()
    {
        foreach (var player in playersInZone)
        {
            player.UnregisterSlowZone(this);
        }
        playersInZone.Clear();
    }
}