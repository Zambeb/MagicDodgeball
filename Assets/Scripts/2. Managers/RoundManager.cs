using System.Collections;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void OnPlayerDied(PlayerController deadPlayer)
    {
        Debug.Log($"Round over. {deadPlayer.name} lost.");
        StartCoroutine(RespawnAllPlayersAfterDelay(3f));
    }
    
    private IEnumerator RespawnAllPlayersAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        PlayerController[] allPlayers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        foreach (PlayerController player in allPlayers)
        {
            player.ResetCharacter();
        }
    }
}

