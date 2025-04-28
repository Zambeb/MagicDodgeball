using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }
    [SerializeField] private UpgradeScreen upgradeScreen;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {

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
        //upgradeScreen.GameObject().SetActive(true);
        //upgradeScreen.Open(player);
        
        
        foreach (PlayerController player in allPlayers)
        {
            player.ResetCharacter();
        }
        
    }
}

