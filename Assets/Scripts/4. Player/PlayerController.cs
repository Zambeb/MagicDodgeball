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

    private Vector2 movementInput;
    private Vector2 aimInput;
    private Vector3 moveDir;

    private Camera mainCamera;
    private PlayerInput playerInput;
    private PlayerGun gun;
    private CharacterController controller;

    public string currentControlScheme;

    private bool disabled;
    private bool invincible;

    public List<UpgradeEffectBase> acquiredUpgrades = new List<UpgradeEffectBase>();
    public UpgradeEffectBase acquiredActiveAbility;

    public ShieldOrbitManager shieldOrbit;

    void Awake()
    {
        mainCamera = Camera.main;
        
        gun = GetComponent<PlayerGun>();
        controller = GetComponent<CharacterController>();
        
        initialStats = new PlayerStats(stats);
        //speed = stats.moveSpeed;
    }

    private void Start()
    {
        playerInput = GetComponentInChildren<PlayerInput>();
        disabled = true;
        invincible = false;
        currentControlScheme = playerInput.currentControlScheme;
    }

    void Update()
    {
        if (!disabled)
        {
            controller.Move(moveDir * stats.moveSpeed * Time.deltaTime);
            HandleRotation();
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
        moveDir = new Vector3(movementInput.x, 0, movementInput.y);
        
    }
    public void OnAim(InputAction.CallbackContext ctx) 
    {
        aimInput = ctx.ReadValue<Vector2>();
        
    }
    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !disabled)
        {
            gun.Shoot(playerIndex, stats.maxBounces, stats.projectileSpeed, stats.accelerationAfterBounce, stats.canStun, stats.stunDuration);
        }
    }

    public void OnPerformActiveAbility(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !disabled && acquiredActiveAbility != null)
        {
            acquiredActiveAbility.PerformAbility(this);
        }
    }

    public void Dash(float distance, float duration)
    {
        StartCoroutine(PerformDash(distance, duration));
    }

    public IEnumerator PerformDash(float distance, float duration)
    {
        disabled = true;
        invincible = true;

        float elapsed = 0f;
        Vector3 dashDirection = transform.forward.normalized;
        Vector3 startPosition = transform.position;

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
    }
    
    public void TakeDamage()
    {
        if (RoundManager.Instance.roundActive && !invincible)
        {
            RoundManager.Instance.RegisterHit(playerIndex);
            Debug.Log("Ouch!");
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
    
    public void DisableCharacter()
    {
        //playerInput.GameObject().SetActive(false);
        disabled = true;
        invincible = true;
    }

    public void ResetCharacter()
    {
        this.GameObject().transform.position = PlayerSpawner.instance.spawnPoints[playerIndex].position;
        //playerInput.GameObject().SetActive(true);
        shieldOrbit.ClearShields();
        disabled = false;
        invincible = false;
        ApplyAllUpgrades();
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
        foreach (var upgrade in acquiredUpgrades)
        {
            upgrade.Apply(this);
        }
    }
}
