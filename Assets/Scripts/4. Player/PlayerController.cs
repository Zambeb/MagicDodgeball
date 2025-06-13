using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageable
{
    public int playerIndex;
    [SerializeField] public PlayerStats stats;
    private PlayerStats initialStats;
    //private float speed = 5f;
    public float rotationSpeed = 720f;
    [SerializeField] private CharacterVisuals _visuals;

    private Vector2 movementInput;
    private Vector2 aimInput;
    private Vector3 moveDir;

    private Camera mainCamera;
    private PlayerInput playerInput;
    public PlayerGun gun;
    private CharacterController controller;

    public string currentControlScheme;

    private bool disabled;
    private bool invincible;
    private bool canShoot;
    private bool activeApplied = false;

    public List<UpgradeEffectBase> acquiredUpgrades = new List<UpgradeEffectBase>();
    public UpgradeEffectBase acquiredActiveAbility;
    public List<UpgradeEffectBase> acquiredUpgradeEffectsPrefabs = new List<UpgradeEffectBase>();
    public UpgradeEffectBase acquiredActiveAbilityPrefab;
    public bool IsActiveOnCooldown => activeApplied;
    public float activeCooldownTimeRemaining;
    public float activeCooldownDuration;
    public CooldownRing cooldownRing;

    public ShieldOrbitManager shieldOrbit;
    
    private PlayerAnimatorController animController;

    [Header("Mouse Rotation")]
    [SerializeField] private LayerMask groundMask; 
    [SerializeField] private float rotationSmoothness = 15f;
    [SerializeField] private float raycastUpdateInterval = 0.05f; 
    private float lastRaycastTime;

    void Awake()
    {
        mainCamera = Camera.main;
        
        gun = GetComponent<PlayerGun>();
        controller = GetComponent<CharacterController>();
        animController = GetComponentInChildren<PlayerAnimatorController>();
        
        initialStats = new PlayerStats(stats);
        //speed = stats.moveSpeed;
    }

    private void Start()
    {
        playerInput = GetComponentInChildren<PlayerInput>();
        disabled = true;
        invincible = false;
        canShoot = true;
        currentControlScheme = playerInput.currentControlScheme;
    }

    void Update()
    {
        if (!disabled)
        {
            controller.Move(moveDir * stats.moveSpeed * Time.deltaTime);
        
            // Оптимизация: проверяем, есть ли ввод
            if (aimInput.sqrMagnitude > 0.1f || currentControlScheme != "Gamepad")
            {
                HandleRotation();
            }
        }
    }

    void HandleRotation()
    {
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
            if (Time.time - lastRaycastTime >= raycastUpdateInterval)
            {
                UpdateMouseRotation();
                lastRaycastTime = Time.time;
            }
        }
    }
    
    private void UpdateMouseRotation()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
    
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            Vector3 lookPoint = hit.point;
            lookPoint.y = transform.position.y; 
            
            Vector3 direction = (lookPoint - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * Time.deltaTime);
            }
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
        moveDir = new Vector3(movementInput.x, 0, movementInput.y);
    }
    
    // Method for PlayerAnimatorController to access movement input
    public Vector2 GetMovementInput()
    {
        return movementInput;
    }
    public void OnAim(InputAction.CallbackContext ctx) 
    {
        aimInput = ctx.ReadValue<Vector2>();
        
    }
    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !disabled && canShoot && gun.activeProjectiles.Count < stats.maxProjectiles)
        {
            gun.Shoot(playerIndex, stats.maxBounces, stats.projectileSpeed, stats.accelerationAfterBounce, stats.canStun, stats.stunDuration);
            
            // Trigger attack animation
            if (animController != null)
            {
                animController.TriggerAttackAnimation();
            }
        }
    }

    public void OnPerformActiveAbility(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (acquiredActiveAbility == null) return;

            if (!disabled && !IsActiveOnCooldown)
            {
                acquiredActiveAbility.PerformAbility(this);
            }
            else
            {
                cooldownRing.ShowCooldown(activeCooldownTimeRemaining, activeCooldownDuration);
            }
        }
    }

    public void Dash(float distance, float duration, float cooldown)
    {
        if (!activeApplied)
        {
            StartCoroutine(PerformDash(distance, duration, cooldown));
        }
    }

    public void ForceField(float duration, float speedMultiplier, float cooldown)
    {
        if (!activeApplied)
        {
            StartCoroutine(PerformForceField(duration, speedMultiplier, cooldown));
        }
        
    }

    public IEnumerator PerformDash(float distance, float duration, float cooldown)
    {
        disabled = true;
        invincible = true;

        activeApplied = true;

        float elapsed = 0f;
        Vector3 dashDirection = moveDir.normalized;
        
        if (dashDirection.sqrMagnitude < 0.01f)
            dashDirection = transform.forward;

        while (elapsed < duration)
        {
            float delta = Time.deltaTime;
            elapsed += delta;

            float moveStep = (distance / duration) * delta;
            controller.Move(dashDirection * moveStep);

            yield return null;
        }

        disabled = false;
        invincible = false;
        
        activeCooldownDuration = cooldown;
        activeCooldownTimeRemaining = cooldown;
        
        while (activeCooldownTimeRemaining > 0)
        {
            activeCooldownTimeRemaining -= Time.deltaTime;
            yield return null;
        }

        activeApplied = false;
    }

    public IEnumerator PerformForceField(float duration, float speedMultiplier, float cooldown)
    {
        invincible = true;
        canShoot = false;
        activeApplied = true;
        
        float initialSpeed = stats.moveSpeed;
        stats.moveSpeed *= speedMultiplier;

        yield return new WaitForSeconds(duration);
        
        stats.moveSpeed = initialSpeed;
        canShoot = true;
        invincible = false;
        
        activeCooldownDuration = cooldown;
        activeCooldownTimeRemaining = cooldown;

        while (activeCooldownTimeRemaining > 0)
        {
            activeCooldownTimeRemaining -= Time.deltaTime;
            yield return null;
        }

        activeApplied = false;
    }
    
    public void TakeDamage()
    {
        if (RoundManager.Instance.roundActive && !invincible)
        {
            RoundManager.Instance.RegisterHit(playerIndex);
            Debug.Log("Ouch!");
            _visuals.FlashWhite(3, 0.5f);
            if (stats.immunityAfterHit != 0)
            {
                StartCoroutine(InvinvibityAfterHit(stats.immunityAfterHit));
            }
        }
    }
    
    public void Stun(float duration)
    {
        if (RoundManager.Instance.roundActive && !invincible)
        {
            Debug.Log("Stunned!");
            StartCoroutine(PerformStun(duration));
        }
    }
    
    private IEnumerator PerformStun(float duration)
    {
        disabled = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (RoundManager.Instance.roundActive)
        {
            disabled = false;
        }
    }

    private IEnumerator InvinvibityAfterHit(float duration)
    {
        invincible = true;
        Debug.Log("Invincibility starts");
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        invincible = false;
        Debug.Log("Invincibility ends");
    }
    
    public void DisableCharacter()
    {
        disabled = true;
        invincible = true;
    }
    
    public void EnableCharacter()
    {
        disabled = false;
        invincible = false;
    }

    public void ResetCharacter()
    {
        shieldOrbit.ClearShields();
        disabled = false;
        invincible = false;
        ApplyAllUpgrades();
    }

    public void ResetPosition()
    {
        this.GameObject().transform.position = PlayerSpawner.instance.spawnPoints[playerIndex].position;
    }
    
    public void AddUpgrade(UpgradeEffectBase effect)
    {
        acquiredUpgrades.Add(effect);
    }

    public void AddActiveAbility(UpgradeEffectBase effect)
    {
        acquiredActiveAbility = effect;
    }
    
    public void ApplyAllUpgrades()
    {
        stats = new PlayerStats(initialStats);
        for (int i = 0; i < acquiredUpgrades.Count; i++)
        {
            acquiredUpgrades[i].Apply(this);
        }
    }
}
