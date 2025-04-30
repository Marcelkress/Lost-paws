using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Movement))]
public class CatInput : MonoBehaviour
{
    [Header("Jump settings")] 
    public float maxJumpHeight = 4f;
    public float minJumpHeight = 1f;
    public float timeToJumpMax = 0.4f;
    public float coyoteTime = 0.1f;
    public UnityEvent JumpEvent;

    [Header("Movement settings")] 
    public float moveSpeed;
    public float sprintSpeed;
    public float crouchSpeed;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = 0.1f;

    [Header("Wall jump settings")] 
    public bool canWallJump = false;
    public float wallSlideSpeedMax = 3f;
    public float wallStickTime = 0.25f;
    private float timeToWallUnstick;
    public Vector2 wallJumpClimb, wallJumpOff, wallLeap;
    [HideInInspector] public bool wallSliding;
    
    private float gravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private float coyoteTimeCounter;
    
    [HideInInspector] public Vector3 velocity;
    private Movement movement;
    private BoxCollider2D collider;
    
    [HideInInspector] public Vector2 inputVector;
    [HideInInspector] public bool jumpTrigger;
    [HideInInspector] public bool jumpRelease;
    [HideInInspector] public bool sprint;
    [HideInInspector] public bool crouch;

    private float velocityXSmoothing;

    private InputAction jumpAction;
    
    void Start()
    {
        movement = GetComponent<Movement>();
 
        gravity = -(2 * maxJumpHeight / Mathf.Pow(timeToJumpMax, 2));
        maxJumpVelocity = Mathf.Abs(gravity * timeToJumpMax);
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        jumpAction = GetComponent<PlayerInput>().actions.FindAction("Jump");
        jumpAction.performed += JumpPerformed;
        jumpAction.canceled += JumpReleased;
    }

    void FixedUpdate()
    {
        int wallDirX = (movement.collisions.left) ? -1 : 1;
        wallSliding = false;
        
        float targetSpeed;
        if (sprint)
        {
            targetSpeed = sprintSpeed;
        }
        else if(crouch)
        {
            targetSpeed = crouchSpeed;
        }
        else
        {
            targetSpeed = moveSpeed;
        }
        
        float targetVelocityX = inputVector.x * targetSpeed;
        
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing,
            (movement.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne));
        

        if ((movement.collisions.left || movement.collisions.right) && !movement.collisions.below && velocity.y < 0)
        {
            if (canWallJump)
            {
                wallSliding = true;
                
                if (velocity.y < -wallSlideSpeedMax)
                {
                    velocity.y = -wallSlideSpeedMax;
                }

                if (timeToWallUnstick > 0)
                {
                    velocity.x = 0;
                    velocityXSmoothing = 0;
                    
                    if (inputVector.x != wallDirX && inputVector.x != 0)
                    {
                        timeToWallUnstick -= Time.deltaTime;
                    }
                    else
                    {
                        timeToWallUnstick = wallStickTime;
                    }
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
        }
        
        // Update coyote time counter
        if (movement.collisions.below)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        
        if (jumpTrigger)
        {
            if (wallSliding)
            {
                if (wallDirX == inputVector.x)
                {
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;
                }
                else if (inputVector.x == 0)
                {
                    velocity.x = -wallDirX * wallJumpOff.x;
                    velocity.y = wallJumpOff.y;
                }
                else
                {
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                }
            }
            
            // Normal jumping
            if (movement.collisions.below || coyoteTimeCounter > 0)
            {
                velocity.y = maxJumpVelocity;
                coyoteTimeCounter = 0;
            }
            
            jumpTrigger = false;
        }

        if (jumpRelease)
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
            jumpRelease = false;
        }
        
        // Movement stuff
        velocity.y += gravity * Time.deltaTime;
        
        movement.Move(velocity * Time.deltaTime, inputVector);
        
        if (movement.collisions.above || movement.collisions.below)
        {
            velocity.y = 0;
        }
    }

    private void JumpPerformed(InputAction.CallbackContext context)
    {
        JumpEvent.Invoke();
        jumpTrigger = !jumpTrigger;
    }

    private void JumpReleased(InputAction.CallbackContext context)
    {
        jumpRelease = true;
    }
    
    public void OnMove(InputValue value)
    {
        Vector2 vector = value.Get<Vector2>();

        if (Mathf.Abs(vector.x) > .7f)
            inputVector = new(1 * Mathf.Sign(vector.x), vector.y);
        else
            inputVector = vector;

    }
    
    public void OnJump(InputValue value)
    {
        jumpTrigger = value.isPressed;
    }
    
    public void OnSprint(InputValue value)
    {
        sprint = value.isPressed;
    }

    public void OnCrouch(InputValue value)
    {
        crouch = value.isPressed;
    }
}