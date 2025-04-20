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
    }
}

