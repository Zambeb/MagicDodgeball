using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageable
{
    public int playerIndex;
    public PlayerController opponent;
    [SerializeField] public PlayerStats stats;
    private PlayerStats initialStats;
    private float speedMultiplier = 1f;
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
    public ChargingCircle chargingCircle;

    public ShieldOrbitManager shieldOrbit;
    
    private PlayerAnimatorController animController;

    [Header("Mouse Rotation")]
    [SerializeField] private LayerMask groundMask; 
    [SerializeField] private float rotationSmoothness = 15f;
    [SerializeField] private float raycastUpdateInterval = 0.05f; 
    private float lastRaycastTime;
    
    private readonly HashSet<TrailSlowZone> activeSlowZones = new HashSet<TrailSlowZone>();

    [Header("Charged Shot")] 
    private float chargeMultiplier = 1f;
    private bool isCharging = false;
    private float chargeTime = 0f;
    private bool chargeInputHeld = false;

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
        if (!disabled && controller.enabled)
        {
            controller.Move(moveDir * stats.moveSpeed * speedMultiplier * Time.deltaTime);
            
            if (aimInput.sqrMagnitude > 0.1f || currentControlScheme != "Gamepad")
            {
                HandleRotation();
            }
            
            if (isCharging)
            {
                chargeTime += Time.deltaTime;
                chargeTime = Mathf.Min(chargeTime, stats.maxChargeTime);
                chargeMultiplier = 1f + (chargeTime * stats.chargeSpeedIncreasePerSecond);
                //Debug.Log("CharMult = " + chargeMultiplier);
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
        if (movementInput.magnitude > 1f)
        {
            movementInput = movementInput.normalized;
        }
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
        if (disabled || !canShoot || gun.activeProjectiles.Count >= stats.maxProjectiles)
            return;

        if (stats.canCharge)
        {
            if (ctx.started && !chargeInputHeld)
            {
                chargeTime = 0f;
                chargeMultiplier = 1f;
                isCharging = true;
                chargeInputHeld = true;
                _visuals.ChargingVFXOn(stats.maxChargeTime);

                Debug.Log("Start charging at multiplier = " + chargeMultiplier);
                if (chargingCircle != null)
                {
                    chargingCircle.Show(stats.maxChargeTime);
                }
            }
            else if (ctx.canceled)
            {
                float val = ctx.ReadValue<float>();
                if (val > 0.1f)
                {
                    return;
                }
                if (isCharging)
                {
                    if (chargingCircle != null)
                    {
                        chargingCircle.Hide();
                    }
                    ShootCharged();
                    chargeInputHeld = false;
                    if (animController != null)
                    {
                        animController.TriggerAttackAnimation();
                    }
                }
                
            }
        }
        else
        {
            if (ctx.performed)
            {
                ShootNormal();
                if (animController != null)
                {
                    animController.TriggerAttackAnimation();
                }
            }
        }
    }
    
    private void ShootNormal()
    {
        gun.Shoot(playerIndex, stats.maxBounces, stats.projectileSpeed, 
            stats.accelerationAfterBounce, stats.canStun, 
            stats.stunDuration, stats.leavesTrail);
        UpdateBallsDisplay();
        Debug.Log("Shot Normal");
    }
    
    private void ShootCharged()
    {
        float finalSpeed = stats.projectileSpeed * chargeMultiplier;
        gun.Shoot(playerIndex, stats.maxBounces, finalSpeed, 
            stats.accelerationAfterBounce, stats.canStun, 
            stats.stunDuration, stats.leavesTrail);
        UpdateBallsDisplay();
        _visuals.ChargingVFXOff();
        Debug.Log("Shot Charged with multiplier = " + chargeMultiplier);
        ResetChargeState();
    }
    
    private void UpdateBallsDisplay()
    {
        int shotBalls = gun.activeProjectiles.Count;
        int notShotBalls = stats.maxProjectiles - shotBalls;
        UIManager.Instance.UpdateBallsDisplay(playerIndex, notShotBalls, shotBalls);
    }

    private void ResetChargeState()
    {
        isCharging = false;
        _visuals.ChargingVFXOff();
        chargeTime = 0f;
        chargeMultiplier = 1f;
    }
    
    public void TakeDamage()
    {
        if (RoundManager.Instance.roundActive && !invincible)
        {
            if (animController != null) animController.TriggerGetHitAnimation();
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
            _visuals.StunVisualEffect(duration);
        }
    }
    
    private IEnumerator PerformStun(float duration)
    {
        if (animController != null) animController.TriggerStunAnimation();
        disabled = true;
        if (isCharging && chargingCircle != null)
        {
            chargingCircle.Hide();
        }
        chargeInputHeld = false;
        isCharging = false;
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

    public void RegisterSlowZone(TrailSlowZone zone, float slowAmount)
    {
        activeSlowZones.Add(zone);
        speedMultiplier = slowAmount;
    }

    public void UnregisterSlowZone(TrailSlowZone zone)
    {
        activeSlowZones.Remove(zone);
        if (activeSlowZones.Count == 0)
            speedMultiplier = 1f;
    }

    private IEnumerator InvinvibityAfterHit(float duration)
    {
        invincible = true;
        _visuals.ImmunityFVX(duration);
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
        if (isCharging && chargingCircle != null)
        {
            chargingCircle.Hide();
        }

        isCharging = false;
        chargeInputHeld = false;
        _visuals.ChargingVFXOff();
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
        chargeInputHeld = false;
        ApplyAllUpgrades();
        UIManager.Instance.UpdateBallsDisplay(playerIndex, stats.maxProjectiles, 0);
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
    
    // ================
    // ACTIVE ABILITIES
    // ================
    
    public void OnPerformActiveAbility(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (acquiredActiveAbility == null) return;

            if (!disabled && !IsActiveOnCooldown)
            {
                acquiredActiveAbility.PerformAbility(this);
                cooldownRing.StartCooldown(activeCooldownDuration);
            }
        }
    }

    public void Dash(float distance, float duration, float cooldown)
    {
        if (animController != null) animController.TriggerDashAnimation();
        if (!activeApplied)
        {
            StartCoroutine(PerformDash(distance, duration, cooldown));
            if (moveDir.sqrMagnitude > 0.01f)
            {
                _visuals.DashVisualEffect(moveDir, duration);
            }
            else
            {
                _visuals.DashVisualEffect(transform.forward, duration);
            }
        }
    }

    public void ForceField(float duration, float speedMultiplier, float cooldown)
    {
        if (!activeApplied)
        {
            StartCoroutine(PerformForceField(duration, speedMultiplier, cooldown));
            _visuals.ForceFieldEffect(duration);
        }
    }

    public void Parry(float radius, float cooldown)
    {
        if (animController != null) animController.TriggerParryAnimation();
        if (!activeApplied)
        {
            if (playerIndex == 0) opponent = RoundManager.Instance.player2;
            else opponent = RoundManager.Instance.player1;
            _visuals.ParryVisualEffect();
            StartCoroutine(PerformParry(radius, cooldown));
        }
    }

    public void Swap(float cooldown, float delay)
    {
        if (!activeApplied)
        {
            var p1 = RoundManager.Instance.player1;
            var p2 = RoundManager.Instance.player2;

            if (p1 == null || p2 == null) return;

            StartCoroutine(PerformSwap(p1, p2, cooldown, delay));
        }
    }

    public IEnumerator PerformDash(float distance, float duration, float cooldown)
    {
        activeApplied = true;
        
        disabled = true;
        invincible = true;
        if (isCharging && chargingCircle != null)
        {
            chargingCircle.Hide();
        }

        isCharging = false;
        chargeInputHeld = false;

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

        if (RoundManager.Instance.roundActive)
        {
            disabled = false;
        }
        
        invincible = false;
        
        SetActiveCooldown(cooldown);
    }

    public IEnumerator PerformForceField(float duration, float speedMultiplier, float cooldown)
    {
        activeApplied = true;
        
        invincible = true;
        canShoot = false;

        float initialSpeed = stats.moveSpeed;
        stats.moveSpeed *= speedMultiplier;

        yield return new WaitForSeconds(duration);
        
        stats.moveSpeed = initialSpeed;
        canShoot = true;
        invincible = false;
        
        SetActiveCooldown(cooldown);
    }

    private IEnumerator PerformParry(float radius, float cooldown)
    {
        activeApplied = true;
        invincible = true; // Needed?
        Vector3 center = transform.position;
        center.y = 1;

        Collider[] hits = Physics.OverlapSphere(center, radius);
        foreach (var hit in hits)
        {
            Projectile proj = hit.GetComponent<Projectile>();
            if (proj != null)
            {
                Vector3 directionToOpponent = (opponent.transform.position - proj.transform.position).normalized;
                proj.Redirect(directionToOpponent);
            }
        }
        
        yield return new WaitForSeconds(0.1f);
        
        invincible = false;

        SetActiveCooldown(cooldown);
    }

    private IEnumerator PerformSwap(PlayerController p1, PlayerController p2, float cooldown, float delay)
    {
        activeApplied = true;
        
        CharacterController cc1 = p1.GetComponent<CharacterController>();
        CharacterController cc2 = p2.GetComponent<CharacterController>();

        if (cc1 != null) cc1.enabled = false;
        if (cc2 != null) cc2.enabled = false;

        GameObject vfx1 = p1._visuals.SwapVisualEffect();
        GameObject vfx2 = p2._visuals.SwapVisualEffect();

        //yield return new WaitForSeconds(delay);
        
        Vector3 p1StartPos = p1.transform.position;
        Vector3 p2StartPos = p2.transform.position;
        Vector3 v1StartPos = vfx1.transform.position;
        Vector3 v2StartPos = vfx2.transform.position;

        Vector3 p1DownPos = p1StartPos + Vector3.down * 2f;
        Vector3 p2DownPos = p2StartPos + Vector3.down * 2f;

        float phaseDuration = delay / 3f; 
        
        float downElapsed = 0f;
        while (downElapsed < phaseDuration)
        {
            downElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(downElapsed / phaseDuration);
            p1.transform.position = Vector3.Lerp(p1StartPos, p1DownPos, t);
            p2.transform.position = Vector3.Lerp(p2StartPos, p2DownPos, t);
            yield return null;
        }
        
        float swapElapsed = 0f;
        while (swapElapsed < phaseDuration)
        {
            swapElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(swapElapsed / phaseDuration);
            p1.transform.position = Vector3.Lerp(p1DownPos, p2DownPos, t);
            p2.transform.position = Vector3.Lerp(p2DownPos, p1DownPos, t);
            vfx1.transform.position = Vector3.Lerp(v1StartPos, v2StartPos, t);
            vfx2.transform.position = Vector3.Lerp(v2StartPos, v1StartPos, t);
            yield return null;
        }
        
        float upElapsed = 0f;
        while (upElapsed < phaseDuration)
        {
            upElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(upElapsed / phaseDuration);
            p1.transform.position = Vector3.Lerp(p2DownPos, p2StartPos, t);
            p2.transform.position = Vector3.Lerp(p1DownPos, p1StartPos, t);
            yield return null;
        }
        
        Quaternion tempRotation = p1.transform.rotation;
        p1.transform.rotation = p2.transform.rotation; 
        p2.transform.rotation = tempRotation;

        if (cc1 != null) cc1.enabled = true;
        if (cc2 != null) cc2.enabled = true;

        SetActiveCooldown(cooldown);
    }

    public void SetActiveCooldown(float cooldownDuration)
    {
        if (cooldownDuration <= 0) return;

        activeCooldownDuration = cooldownDuration;
        activeCooldownTimeRemaining = cooldownDuration;
        activeApplied = true;
    
        StartCoroutine(CooldownRoutine());
    }
    
    private IEnumerator CooldownRoutine()
    {
        while (activeCooldownTimeRemaining > 0)
        {
            activeCooldownTimeRemaining -= Time.deltaTime;
            yield return null;
        }
    
        activeApplied = false;
    }
}
