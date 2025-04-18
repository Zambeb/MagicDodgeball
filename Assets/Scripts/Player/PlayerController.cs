using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    [SerializeField] private float playerSpeed = 5.0f;
    private float gravityValue = -9.81f;

    private Vector3 move;
    private Vector3 aimDirection = Vector3.forward;
    private bool usingMouseAim = false;

    [SerializeField] private Transform aimPivot;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    private bool isKeyboard;  // Переменная для определения типа устройства

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        controller.Move(move * Time.deltaTime * playerSpeed);

        // Gravity
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // Обработка прицеливания в зависимости от устройства
        if (isKeyboard)
        {
            HandleMouseAim();
        }
        else
        {
            HandleGamepadAim();
        }

        if (!usingMouseAim && aimDirection.sqrMagnitude > 0.1f)
        {
            transform.forward = aimDirection;
        }
    }

    public void SetInputDevice(bool keyboard)
    {
        isKeyboard = keyboard;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 movement = context.ReadValue<Vector2>();
        move = new Vector3(movement.x, 0, movement.y);
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (isKeyboard)
        {
            // Управление мышью
            Vector2 aimInput = context.ReadValue<Vector2>();
            Vector3 newAim = new Vector3(aimInput.x, 0, aimInput.y);
            if (newAim.sqrMagnitude > 0.1f)
            {
                aimDirection = newAim.normalized;
                usingMouseAim = true;
            }
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PlayerGun.Instance.Shoot();
        }
    }

    private void HandleMouseAim()
    {
        if (Mouse.current == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 lookPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(lookPoint);
            usingMouseAim = true;
        }
    }

    private void HandleGamepadAim()
    {
        if (Gamepad.current == null) return;

        Vector2 aimInput = Gamepad.current.rightStick.ReadValue();
        Vector3 newAim = new Vector3(aimInput.x, 0, aimInput.y);
        if (newAim.sqrMagnitude > 0.1f)
        {
            aimDirection = newAim.normalized;
        }
    }
}