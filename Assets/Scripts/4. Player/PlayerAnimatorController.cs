using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;

    // Animation Parameters
    private const string IS_RUNNING_FORWARD = "isRunningForward";
    private const string IS_RUNNING_LEFT = "isRunningLeft";
    private const string IS_RUNNING_RIGHT = "isRunningRight";
    private const string IS_RUNNING_BACKWARD = "isRunningBackward";
    private const string IS_ATTACKING = "isAttacking";
    private const string GET_HIT = "isGettingHit";
    private const string IS_STUNNED = "isStunned";
    private const string WON = "Won";
    private const string LOST = "Lost";
    private const string IS_DASHING = "isDashing";
    private const string IS_PARRYING = "isParrying";

    void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponentInParent<PlayerController>();
        
        if (playerController == null)
        {
            Debug.LogError("PlayerAnimatorController requires a PlayerController component in parent!");
        }
        // Reset all animation parameters
        ResetAnimationParameters();

    }

    void Update()
    {
        if (playerController != null)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        // Get movement input from PlayerController
        Vector2 movementInput = playerController.GetMovementInput();
        
        // Reset all movement parameters first
        ResetMovementParameters();
        
        // Skip if no movement
        if (movementInput.magnitude < 0.1f)
            return;
            
        // Convert the 2D movement input to a 3D vector in world space
        Vector3 movementVector = new Vector3(movementInput.x, 0, movementInput.y);
        
        // Convert world space movement to local space relative to character's forward direction
        Vector3 localMovement = transform.parent.InverseTransformDirection(movementVector);
        
        // Determine movement direction relative to character's facing direction
        if (localMovement.z > 0.1f) // Forward relative to character facing
        {
            animator.SetBool(IS_RUNNING_FORWARD, true);
         //   Debug.Log("Running Forward (Gun Direction)");
        }
        else if (localMovement.z < -0.1f) // Backward relative to character facing
        {
            animator.SetBool(IS_RUNNING_BACKWARD, true);
           // Debug.Log("Running Backward (Away from Gun)");
        }
        
        if (localMovement.x > 0.1f) // Right relative to character facing
        {
            animator.SetBool(IS_RUNNING_RIGHT, true);
           // Debug.Log("Running Right (Gun's Right)");
        }
        else if (localMovement.x < -0.1f) // Left relative to character facing
        {
            animator.SetBool(IS_RUNNING_LEFT, true);
           // Debug.Log("Running Left (Gun's Left)");
        }
    }

    // This method will be called from PlayerController when firing
     public void TriggerGetHitAnimation()
    {
        Debug.Log("Animator: GetHit " + animator.GetInstanceID() + ", Name: " + animator.gameObject.name);
        animator.SetTrigger(GET_HIT);
    }

    public void TriggerStunAnimation()
    {
        animator.SetTrigger(IS_STUNNED);
    }

    public void TriggerWonAnimation()
    {
        animator.SetTrigger(WON);
    }

    public void TriggerLostAnimation()
    {
        animator.SetTrigger(LOST);
      //  Debug.Log("Triggered LostAnimation");
    }

    public void TriggerDashAnimation()
    {
        animator.SetTrigger(IS_DASHING);
       // Debug.Log("Triggered DashAnimation");
    }

    public void TriggerParryAnimation()
    {
        animator.SetTrigger(IS_PARRYING);
        //Debug.Log("Triggered ParryAnimation");
    }

    public void TriggerAttackAnimation()
    {
        Debug.Log("Animator: Attack " + animator.GetInstanceID() + ", Name: " + animator.gameObject.name);
        animator.SetTrigger(IS_ATTACKING); 
    }

    private void ResetAnimationParameters()
    {
        ResetMovementParameters();
        animator.ResetTrigger(IS_ATTACKING);
        animator.ResetTrigger(GET_HIT);
        animator.ResetTrigger(IS_STUNNED);
        animator.ResetTrigger(WON);
        animator.ResetTrigger(LOST);
        animator.ResetTrigger(IS_DASHING);
        animator.ResetTrigger(IS_PARRYING);
    }

    private void ResetMovementParameters()
    {
        animator.SetBool(IS_RUNNING_FORWARD, false);
        animator.SetBool(IS_RUNNING_LEFT, false);
        animator.SetBool(IS_RUNNING_RIGHT, false);
        animator.SetBool(IS_RUNNING_BACKWARD, false);
    }
}
