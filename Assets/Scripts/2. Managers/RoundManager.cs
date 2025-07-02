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
    [SerializeField] private float initialRoundDuration = 30f;
    [SerializeField] private float delayBeforeUpgrades = 3f;
    //[SerializeField] private UpgradeScreen upgradeScreen;
    public int roundCount;
    public int twoBuffRound = 4;
    public int longerRoundsRound = 3;
    public int projectileCount = 0;
    private float roundDuration;

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
    public int playersReady = 0;
    private int playersSelectedUpgrade;
    
    [Header("Logger")]
    private PlayerUpgradeLogger upgradeLogger;

    private bool endRoundSoundPlayed;

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
        roundDuration = initialRoundDuration;
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
        
        if (!endRoundSoundPlayed && timer <= 3f)
        {
            endRoundSoundPlayed = true;
            SoundManager.Instance.PlaySFX("RoundEnding");
        }
        
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
            SoundManager.Instance.FadeMusicVolume(0.0f, 1f);
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
        endRoundSoundPlayed = false;
        
        UIManager.Instance.UpdateRoundPoints(player1points, player2points);
        if (roundCount > longerRoundsRound)
        {
            roundDuration += 5f;
        }
        else
        {
            roundDuration = initialRoundDuration;
        }

        StartCoroutine(ResetRoundAfterDelay());
        //Cursor.visible = false;
    }
    
    public void EndRound()
    {
        Debug.Log("Round Ended!");
        roundActive = false;

        player1.DisableCharacter();
        player2.DisableCharacter();
        
        SoundManager.Instance.FadeMusicVolume(0.5f, 1f);

        playersSelectedUpgrade = 0;

        string winnerMessage = "Draw!";
        PlayerController winner = null;
        PlayerController loser = null;

        if (player1points > player2points)
        {
            player1Wins++;
            winnerMessage = "Player 1 has won!";
            winner = player1;
            loser = player2;
            upgradeLogger.LogRound(roundCount, player1, player2, 0);
        }
        else if (player2points > player1points)
        {
            player2Wins++;
            winnerMessage = "Player 2 has won!";
            winner = player2;
            loser = player1;
            upgradeLogger.LogRound(roundCount, player1, player2, 1);
        }
        else
        {
            upgradeLogger.LogRound(roundCount, player1, player2, 2);
        }

        if (winner != null && loser != null)
        {
            PlayerAnimatorController winnerAnim = winner.GetComponentInChildren<PlayerAnimatorController>();
            PlayerAnimatorController loserAnim = loser.GetComponentInChildren<PlayerAnimatorController>();

            if (winnerAnim != null) winnerAnim.TriggerWonAnimation();
            if (loserAnim != null) loserAnim.TriggerLostAnimation();
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
        
        // Destroy all walls left
        MagicBarrier[] allBarrierAbilities = FindObjectsByType<MagicBarrier>(FindObjectsSortMode.None);
        foreach (var barrierAbility in allBarrierAbilities)
        {
            barrierAbility.RemoveAllBarriers();
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
        SoundManager.Instance.PlaySFX("Countdown");

        string[] countdownSteps = { "3", "2", "1"};
        
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

        Debug.Log("Round Started!");
        timer = roundDuration;
        roundActive = true;

        player1.ResetCharacter();
        player2.ResetCharacter();

        if (roundCount == 1 || roundCount == 2)
        {
            SoundManager.Instance.PlayMusic("ArenaLVL1");
        }
        else if (roundCount == 3 || roundCount == 4)
        {
            SoundManager.Instance.PlayMusic("ArenaLVL2");
        }
        else if (roundCount >= 5)
        {
            SoundManager.Instance.PlayMusic("ArenaLVL3");
        }
        
        UIManager.Instance.countdownText.text = "GO!";
        yield return new WaitForSeconds(stepDelay);
        UIManager.Instance.countdownText.gameObject.SetActive(false);
        
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

