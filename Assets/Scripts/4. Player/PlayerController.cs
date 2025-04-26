using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageable
{
    public int playerIndex;
    [SerializeField] public PlayerStats stats;
    [SerializeField] private PlayerStats initialStats;
    //private float speed = 5f;
    public float rotationSpeed = 720f;

    private Vector2 movementInput;
    private Vector2 aimInput;
    private Vector3 move;

    private Camera mainCamera;
    private PlayerInput playerInput;
    private PlayerGun gun;
    private CharacterController controller;
    
    public List<UpgradeEffectBase> acquiredUpgrades = new List<UpgradeEffectBase>();

    [SerializeField] private GameObject[] models;
    [SerializeField] private GameObject deathEffect;

    public bool IsDead => stats.currentHealth <= 0;

    void Awake()
    {
        mainCamera = Camera.main;
        
        gun = GetComponent<PlayerGun>();
        controller = GetComponent<CharacterController>();
        
        initialStats = new PlayerStats(stats);
        stats.ResetHealth();
        //speed = stats.moveSpeed;
    }

    private void Start()
    {
        playerInput = GetComponentInChildren<PlayerInput>();
        UIManager.Instance.InitPlayerHearts(playerIndex, stats.maxHealth);

        Debug.Log($"Initial stats - Health: {initialStats.maxHealth}, Speed: {initialStats.moveSpeed}");
        Debug.Log($"Current stats - Health: {stats.maxHealth}, Speed: {stats.moveSpeed}");
        
    }

    void Update()
    {
        controller.Move(move * stats.moveSpeed * Time.deltaTime);
        HandleRotation();
    }

    void HandleRotation()
    {
        string currentControlScheme = playerInput.currentControlScheme;

        if (currentControlScheme == "Gamepad")
        {
            Vector3 direction = new Vector3(aimInput.x, 0, aimInput.y);
            if (direction.sqrMagnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                Vector3 lookPoint = hit.point;
                Vector3 direction = lookPoint - transform.position;
                direction.y = 0f;

                if (direction.sqrMagnitude > 0.01f)
                {
                    Quaternion rot = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, 15f * Time.deltaTime);
                }
            }
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
        move = new Vector3(movementInput.x, 0, movementInput.y);
        
    }
    public void OnAim(InputAction.CallbackContext ctx) 
    {
        aimInput = ctx.ReadValue<Vector2>();
        
    }
    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            gun.Shoot(playerIndex, stats.maxBounces, stats.projectileSpeed);
        }
    }

    public void TakeDamage(int amount)
    {
        stats.currentHealth -= amount;
        UIManager.Instance.UpdatePlayerHearts(playerIndex, stats.currentHealth);
        Debug.Log("Ouch!");
        if (IsDead)
        {
            Debug.Log($"{gameObject.name} is dead!");
            Die();
        }
    }

    private void Die()
    {
        foreach (var model in models)
        {
            model.SetActive(false);
        }
        deathEffect.SetActive(true);
        playerInput.GameObject().SetActive(false);
        
        if (RoundManager.Instance != null)
        {
            RoundManager.Instance.OnPlayerDied(this);
        }
        else
        {
            Debug.LogError("RoundManager.Instance is null!");
        }
        
    }

    public void ResetCharacter()
    {
        foreach (var model in models)
        {
            model.SetActive(true);
        }
        deathEffect.SetActive(false);
        playerInput.GameObject().SetActive(true);
        ApplyAllUpgrades();
        stats.ResetHealth();
        UIManager.Instance.InitPlayerHearts(playerIndex, stats.maxHealth);
    }
    
    public void AddUpgrade(UpgradeEffectBase effect)
    {
        acquiredUpgrades.Add(effect);
    }
    
    public void ApplyAllUpgrades()
    {
        stats = new PlayerStats(initialStats);
        foreach (var upgrade in acquiredUpgrades)
        {
            upgrade.Apply(this);
        }
    }
}
