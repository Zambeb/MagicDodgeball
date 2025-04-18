using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 720f;

    private Vector2 movementInput;
    private Vector2 aimInput;
    private Vector3 move;

    private Camera mainCamera;
    private PlayerInput playerInput;
    private PlayerGun gun;
    private CharacterController controller;

    void Awake()
    {
        mainCamera = Camera.main;
        
        gun = GetComponent<PlayerGun>();
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        playerInput = GetComponentInChildren<PlayerInput>();
    }

    void Update()
    {
        controller.Move(move * speed * Time.deltaTime);
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
            gun.Shoot();
    }
}
