using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;

    // Animation Parameters
    private const string IS_RUNNING_FORWARD = "isRunningForward";
    private const string IS_RUNNING_LEFT = "isRunningLeft";
    private const string IS_RUNNING_RIGHT = "isRunningRight";
    private const string IS_RUNNING_BACKWARD = "isRunningBackward";
    private const string IS_ATTACKING = "isAttacking";

    void Start()
    {
        animator = GetComponent<Animator>();
        
        // Reset all animation parameters
        ResetAnimationParameters();
    }

    void Update()
    {
        HandleMovement();
        HandleAttack();
    }

    private void HandleMovement()
    {
        // Get movement input from PlayerControls
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Reset all movement parameters first
        ResetMovementParameters();

        // Only set animation parameters if actually moving
        if (vertical > 0.1f) // Forward
        {
            animator.SetBool(IS_RUNNING_FORWARD, true);
            Debug.Log("Moving Forward");
        }
        else if (vertical < -0.1f) // Backward
        {
            animator.SetBool(IS_RUNNING_BACKWARD, true);
            Debug.Log("Moving Backward");
        }

        if (horizontal > 0.1f) // Right
        {
            animator.SetBool(IS_RUNNING_RIGHT, true);
            Debug.Log("Moving Right");
        }
        else if (horizontal < -0.1f) // Left
        {
            animator.SetBool(IS_RUNNING_LEFT, true);
            Debug.Log("Moving Left");
        }

        // If not moving at all, reset all parameters
        if (Mathf.Abs(horizontal) < 0.1f && Mathf.Abs(vertical) < 0.1f)
        {
            ResetMovementParameters();
        }
    }

    private void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger(IS_ATTACKING);
            Debug.Log("Attacking!");
        }
    }

    private void ResetAnimationParameters()
    {
        ResetMovementParameters();
        animator.ResetTrigger(IS_ATTACKING);
    }

    private void ResetMovementParameters()
    {
        animator.SetBool(IS_RUNNING_FORWARD, false);
        animator.SetBool(IS_RUNNING_LEFT, false);
        animator.SetBool(IS_RUNNING_RIGHT, false);
        animator.SetBool(IS_RUNNING_BACKWARD, false);
    }
}
