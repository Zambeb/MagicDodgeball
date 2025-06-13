using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject inRoundUI;

    [Header("Round Points")] [SerializeField]
    private TextMeshProUGUI player1PointsText;

    [SerializeField] private TextMeshProUGUI player2PointsText;

    [Header("Player Balls")] 
    [SerializeField] private GameObject player1Balls;
    [SerializeField] private GameObject player2Balls;
    [SerializeField] private GameObject activeBallUIPrefab;
    [SerializeField] private GameObject notActiveBallUIPrefab;

    [Header("Player Wins")] public VictoryDisplayUI victoryDisplayUI;
    //public TextMeshProUGUI player1WinsText;
    //public TextMeshProUGUI player2WinsText;

    [Header("Upgrade Screens")] [SerializeField]
    private GameObject upgradeScreenMode;

    [SerializeField] private UpgradeScreen player1UpgradeScreen;
    [SerializeField] private UpgradeScreen player2UpgradeScreen;
    [SerializeField] private AcquiredBuffsShower player1Acquired;
    [SerializeField] private AcquiredBuffsShower player2Acquired;
    [SerializeField] private float upgradeSelectionTime = 10f; // в секундах
    [SerializeField] private TextMeshProUGUI upgradeTimerText;
    private float upgradeTimer;
    private bool isUpgradeTimerRunning;

    [Header("Other UI")] public TextMeshProUGUI countdownText;
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
                if (upgradeTimer <= 0f || 
                    (player1UpgradeScreen.HasFinishedChoosing() && 
                     player2UpgradeScreen.HasFinishedChoosing() &&
                     !player1UpgradeScreen.IsAnimating() && 
                     !player2UpgradeScreen.IsAnimating()))
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

    public void UpdateBallsDisplay(int playerIndex, int notShotBalls, int shotBalls)
    {
        Transform targetContainer = null;

        if (playerIndex == 0)
        {
            targetContainer = player1Balls.transform;
        }
        else if (playerIndex == 1)
        {
            targetContainer = player2Balls.transform;
        }
        else
        {
            return;
        }
        
        foreach (Transform child in targetContainer)
        {
            Destroy(child.gameObject);
        }

        if (playerIndex == 0)
        {
            for (int i = 0; i < notShotBalls; i++)
            {
                Instantiate(activeBallUIPrefab, targetContainer);
            }

            for (int i = 0; i < shotBalls; i++)
            {
                Instantiate(notActiveBallUIPrefab, targetContainer);
            }
        }
        else if (playerIndex == 1)
        {
            for (int i = 0; i < shotBalls; i++)
            {
                Instantiate(notActiveBallUIPrefab, targetContainer);
            }

            for (int i = 0; i < notShotBalls; i++)
            {
                Instantiate(activeBallUIPrefab, targetContainer);
            }
        }
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
        StartCoroutine(ForceChooseCoroutine());
    }

    private IEnumerator ForceChooseCoroutine()
    {
        bool p1Done = false;
        bool p2Done = false;

        while (!p1Done || !p2Done)
        {
            if (!p1Done && !player1UpgradeScreen.HasFinishedChoosing())
            {
                player1UpgradeScreen.ChooseRandomUpgrades();
            }
            else
            {
                p1Done = true;
            }

            if (!p2Done && !player2UpgradeScreen.HasFinishedChoosing())
            {
                player2UpgradeScreen.ChooseRandomUpgrades();
            }
            else
            {
                p2Done = true;
            }
            
            yield return null;
        }
        
        UIManager.Instance.CloseUpgradeScreens();
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