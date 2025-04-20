using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Transform[] playerPanels; 
    [SerializeField] private GameObject heartPrefab;   

    private Dictionary<int, List<GameObject>> playerHearts = new Dictionary<int, List<GameObject>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void InitPlayerHearts(int playerIndex, int maxHealth)
    {
        if (playerIndex < 0 || playerIndex >= playerPanels.Length) return;

        List<GameObject> hearts = new List<GameObject>();
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, playerPanels[playerIndex]);
            hearts.Add(heart);
        }

        playerHearts[playerIndex] = hearts;
    }

    public void UpdatePlayerHearts(int playerIndex, int currentHealth)
    {
        if (!playerHearts.ContainsKey(playerIndex)) return;

        List<GameObject> hearts = playerHearts[playerIndex];
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].SetActive(i < currentHealth);
        }
    }
}