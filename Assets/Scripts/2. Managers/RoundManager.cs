using System;
using System.Collections;
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

    [Header("Players")]
    public PlayerController player1;
    public PlayerController player2;

    [Header("Round Points")] 
    public int player1points;
    public int player2points;
    
    private float timer;
    public bool roundActive = false;
    private int playersReady = 0;
    private int playersSelectedUpgrade;
    
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    
    private void Start()
    {
        PlayerSpawner.instance.PlayerJoinedGame += OnPlayerJoined;
        playersSelectedUpgrade = 0;
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
    
    public void StartRound()
    {
        Debug.Log("Round Started!");
        timer = roundDuration;
        roundActive = true;

        player1.ResetCharacter();
        player2.ResetCharacter();

        player1points = 0;
        player2points = 0;
        
        UIManager.Instance.UpdateRoundPoints(player1points, player2points);
    }
    
    public void EndRound()
    {
        Debug.Log("Round Ended!");
        roundActive = false;

        player1.DisableCharacter();
        player2.DisableCharacter();

        playersSelectedUpgrade = 0;
        
        StartCoroutine(ShowUpgradeScreenAfterDelay());
    }
    
    private IEnumerator ShowUpgradeScreenAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeUpgrades);
        UIManager.Instance.OpenUpgradeScreens(player1, player2);
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
        if (playersSelectedUpgrade > 1)
        {
            UIManager.Instance.CloseUpgradeScreens();
            TryStartRound();
        }
    }
}

