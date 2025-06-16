using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }
    
    [Header("Round Settings")]
    [SerializeField] private float roundDuration = 30f;
    [SerializeField] private float delayBeforeUpgrades = 3f;
    [SerializeField] private UpgradeScreen upgradeScreen;
    public int roundCount;
    public int twoBuffRound = 4;
    public int projectileCount = 0;

    [Header("Players")]
    public PlayerController player1;
    public PlayerController player2;

    [Header("Round Points")] 
    public int player1points;
    public int player2points;
    
    [Header("Match Wins")]
    public int player1Wins;
    public int player2Wins;
    private bool gameEnded;

    private float timer;
    public bool roundActive = false;
    private int playersReady = 0;
    private int playersSelectedUpgrade;
    
    [Header("Logger")]
    private PlayerUpgradeLogger upgradeLogger;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        upgradeLogger = gameObject.AddComponent<PlayerUpgradeLogger>();
        upgradeLogger.InitLogSession();
    }
    
    private void Start()
    {
        PlayerSpawner.instance.PlayerJoinedGame += OnPlayerJoined;
        playersSelectedUpgrade = 0;
        gameEnded = false;
        roundCount = 0;
    }
    
    void OnPlayerJoined(PlayerInput input)
    {
        StartCoroutine(WaitForController(input));
    }

    private IEnumerator WaitForController(PlayerInput input)
    {
        yield return null;
        
        PlayerController controller = input.GetComponentInParent<PlayerController>();

        if (input.playerIndex == 0)
        {
            player1 = controller;
            Debug.Log("Player 1 joined");
        }
        else if (input.playerIndex == 1)
        {
            player2 = controller;
            Debug.Log("Player 2 joined");
        }

        playersReady++;

        if (playersReady >= 2)
        {
            TryStartRound();
        }
    }

    private void Update()
    {
        if (!roundActive) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            EndRound();
        }
    }
    
    public void TryStartRound()
    {
        if (player1 != null && player2 != null)
        {
            StartRound();
        }
        else
        {
            Debug.Log("Something wrong, mate");
        }
    }

    public void TryEndRound()
    {
        if (roundActive)
        {
            timer = 0;
        }
    }
    
    public void StartRound()
    {
        Debug.Log("Round " + (roundCount + 1) + " starts");
        roundCount++;
        player1points = 0;
        player2points = 0;
        
        UIManager.Instance.UpdateRoundPoints(player1points, player2points);

        StartCoroutine(ResetRoundAfterDelay());
        //Cursor.visible = false;
    }
    
    public void EndRound()
    {
        Debug.Log("Round Ended!");
        roundActive = false;

        player1.DisableCharacter();
        player2.DisableCharacter();

        playersSelectedUpgrade = 0;

        string winnerMessage = "Draw!";
        if (player1points > player2points)
        {
            player1Wins++;
            winnerMessage = "Player 1 has won!";
            upgradeLogger.LogRound(roundCount, player1, player2, 0);
        }
        else if (player2points > player1points)
        {
            player2Wins++;
            winnerMessage = "Player 2 has won!";
            upgradeLogger.LogRound(roundCount, player1, player2, 1);
        }
        else
        {
            upgradeLogger.LogRound(roundCount, player1, player2, 2);
        }
        
        if (player1Wins >= 4)
        {
            winnerMessage = "PLAYER 1 IS VICTORIOUS!!!";
            upgradeLogger.LogRound(roundCount, player1, player2, 0);
            upgradeLogger.FinalizeLog();

            gameEnded = true;
        }
        else if (player2Wins >= 4)
        {
            winnerMessage = "PLAYER 2 IS VICTORIOUS!!!";
            upgradeLogger.LogRound(roundCount, player1, player2, 1);
            upgradeLogger.FinalizeLog();

            gameEnded = true;
        }
        
        


        UIManager.Instance.ShowWinner(winnerMessage);
        UIManager.Instance.victoryDisplayUI.UpdateVictoryCrystals(player1Wins, player2Wins);

        if (!gameEnded)
        {
            StartCoroutine(ShowUpgradeScreenAfterDelay());
        }
        
        // Destroy all projectiles left
        Projectile[] allProjectiles = FindObjectsByType<Projectile>(FindObjectsSortMode.None);
        foreach (Projectile projectile in allProjectiles)
        {
            projectile.DestroySelf();
        }

        //Cursor.visible = true;
    }
    
    private IEnumerator ShowUpgradeScreenAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeUpgrades);
        UIManager.Instance.HideWinner();
        UIManager.Instance.OpenUpgradeScreens(player1, player2);
    }

    private IEnumerator ResetRoundAfterDelay()
    {
        player1.ResetPosition();
        player2.ResetPosition();
        
        UIManager.Instance.countdownText.gameObject.SetActive(true);

        string[] countdownSteps = { "3", "2", "1", "GO!" };
        
        if (player1Wins > 2 && player2Wins > 2)
        {
            countdownSteps[0] = "Final round!";
        }

        float stepDelay = delayBeforeUpgrades / countdownSteps.Length;

        foreach (string step in countdownSteps)
        {
            UIManager.Instance.countdownText.text = step;
            yield return new WaitForSeconds(stepDelay);
        }

        UIManager.Instance.countdownText.gameObject.SetActive(false);

        Debug.Log("Round Started!");
        timer = roundDuration;
        roundActive = true;
        
        player1.ResetCharacter();
        player2.ResetCharacter();
    }
    
    public void RegisterHit(int damagedIndex)
    {
        if (damagedIndex == 0)
        {
            player2points++;
        }
        else if (damagedIndex == 1)
        {
            player1points++;
        }
        UIManager.Instance.UpdateRoundPoints(player1points, player2points);
    }
    
    public float GetRemainingTime()
    {
        return timer;
    }

    public void PlayerSelectedUpgrade()
    {
        playersSelectedUpgrade++;
        if (playersSelectedUpgrade >= 2)
        {
            UIManager.Instance.CloseUpgradeScreens();
            TryStartRound();
        }
    }
}

