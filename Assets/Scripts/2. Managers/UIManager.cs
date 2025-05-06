using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Round Points")] 
    [SerializeField] private TextMeshProUGUI player1PointsText;
    [SerializeField] private TextMeshProUGUI player2PointsText;

    [Header("Upgrade Screens")] 
    [SerializeField] private UpgradeScreen player1UpgradeScreen;
    [SerializeField] private UpgradeScreen player2UpgradeScreen;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void UpdateRoundPoints(int player1Points, int player2Points)
    {
        player1PointsText.text = player1Points + " Points";
        player2PointsText.text = player2Points + " Points";
    }
    
    public void OpenUpgradeScreens(PlayerController p1, PlayerController p2)
    {
        player1UpgradeScreen.Open(p1);
        player2UpgradeScreen.Open(p2);
    }

    public void CloseUpgradeScreens()
    {
        player1UpgradeScreen.Close();
        player2UpgradeScreen.Close();
    }
}