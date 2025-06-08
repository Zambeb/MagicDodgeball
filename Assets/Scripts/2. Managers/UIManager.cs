using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject inRoundUI;

    [Header("Round Points")] 
    [SerializeField] private TextMeshProUGUI player1PointsText;
    [SerializeField] private TextMeshProUGUI player2PointsText;
    
    [Header("Player Wins")]
    public VictoryDisplayUI victoryDisplayUI;
    //public TextMeshProUGUI player1WinsText;
    //public TextMeshProUGUI player2WinsText;
    
    [Header("Upgrade Screens")] 
    [SerializeField] private GameObject upgradeScreenMode;
    [SerializeField] private UpgradeScreen player1UpgradeScreen;
    [SerializeField] private UpgradeScreen player2UpgradeScreen;
    [SerializeField] private AcquiredBuffsShower player1Acquired;
    [SerializeField] private AcquiredBuffsShower player2Acquired;
    [SerializeField] private float upgradeSelectionTime = 10f; // в секундах
    [SerializeField] private TextMeshProUGUI upgradeTimerText;
    private float upgradeTimer;
    private bool isUpgradeTimerRunning;

    [Header("Other UI")]
    public TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI winnerText;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    
    private void Update()
    {
        if (isUpgradeTimerRunning)
        {
            upgradeTimer -= Time.deltaTime;
            upgradeTimerText.text = Mathf.CeilToInt(upgradeTimer).ToString();

            if (upgradeTimer <= 0f)
            {
                StopUpgradeTimer();
                ForceChooseUpgrades();
            }
            else
            {
                if (player1UpgradeScreen.HasFinishedChoosing() && player2UpgradeScreen.HasFinishedChoosing())
                {
                    StopUpgradeTimer();
                    CloseUpgradeScreens();
                }
            }
        }
    }


    public void UpdateRoundPoints(int player1Points, int player2Points)
    {
        player1PointsText.text = player1Points.ToString();
        player2PointsText.text = player2Points.ToString();
    }
    
    public void OpenUpgradeScreens(PlayerController p1, PlayerController p2)
    {
        upgradeScreenMode.SetActive(true);
        inRoundUI.SetActive(false);

        if (RoundManager.Instance.roundCount >= RoundManager.Instance.twoBuffRound)
        {
            player1UpgradeScreen.Open(p1, 2);
            player2UpgradeScreen.Open(p2, 2);
        }
        else
        {
            player1UpgradeScreen.Open(p1, 1);
            player2UpgradeScreen.Open(p2, 1);
        }
        
        UpdateAllAcquiredBuffs();
        
        StartUpgradeTimer(); 
    }

    public void CloseUpgradeScreens()
    {
        player1UpgradeScreen.Close();
        player2UpgradeScreen.Close();
        upgradeScreenMode.SetActive(false);
        inRoundUI.SetActive(true);
    }
    
    public void ShowWinner(string winnerMessage)
    {
        winnerText.text = winnerMessage;
        winnerText.gameObject.SetActive(true);
    }

    public void HideWinner()
    {
        winnerText.gameObject.SetActive(false);
    }
    
    public void UpdateMatchWins(int p1Wins, int p2Wins)
    {
        //player1WinsText.text = $"{p1Wins} / 4";
        //player2WinsText.text = $"{p2Wins} / 4";
    }

    public void UpdateAllAcquiredBuffs()
    {
        player1Acquired.UpdateBuffIcons();
        player2Acquired.UpdateBuffIcons();
    }
    
    private void StartUpgradeTimer()
    {
        upgradeTimer = upgradeSelectionTime;
        isUpgradeTimerRunning = true;
        upgradeTimerText.gameObject.SetActive(true);
    }
    
    private void StopUpgradeTimer()
    {
        isUpgradeTimerRunning = false;
        upgradeTimerText.gameObject.SetActive(false);
    }
    
    private void ForceChooseUpgrades()
    {
        if (!player1UpgradeScreen.HasFinishedChoosing())
        {
            player1UpgradeScreen.ChooseRandomUpgrades();
        }

        if (!player2UpgradeScreen.HasFinishedChoosing())
        {
            player2UpgradeScreen.ChooseRandomUpgrades();
        }

        //CloseUpgradeScreens();
    }

    public void PauseUpgradeTimer()
    {
        isUpgradeTimerRunning = false;
    }

    public void ResumeUpgradeTimer()
    {
        if (!upgradeScreenMode.activeSelf) return;
        isUpgradeTimerRunning = true;
    }
    
}