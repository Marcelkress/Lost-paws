using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Movement))]
public class CatInput : MonoBehaviour
{
    [TabGroup("Jump")]
    public float maxJumpHeight = 4f;
    [TabGroup("Jump")]
    public float minJumpHeight = 1f;
    [TabGroup("Jump")]
    public float timeToJumpMax = 0.4f;
    [TabGroup("Jump")]
    public float coyoteTime = 0.1f;
    [TabGroup("Jump")]
    public UnityEvent JumpEvent;

    [TabGroup("Movement")]
    public float moveSpeed;
    [TabGroup("Movement")]
    public float sprintSpeed;
    [TabGroup("Movement")]
    public float crouchSpeed;
    [TabGroup("Movement")]
    public float accelerationTimeAirborne = .2f;
    [TabGroup("Movement")]
    public float accelerationTimeGrounded = 0.1f;

    [TabGroup("Wall Jump")]
    public bool canWallJump = false;
    [TabGroup("Wall Jump")]
    public float wallSlideSpeedMax = 3f;
    [TabGroup("Wall Jump")]
    public float wallStickTime = 0.25f;
    [TabGroup("Wall Jump")]
    private float timeToWallUnstick;
    [TabGroup("Wall Jump")] 
    public Vector2 wallJumpClimb, wallJumpOff, wallLeap;
    [HideInInspector] public bool wallSliding;
    
    private float gravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private float coyoteTimeCounter;
    private float lastSpeed;
    private float xSpeed;
    
    [HideInInspector] public Vector3 velocity;
    private Movement movement;
    private BoxCollider2D collider;
    
    [ReadOnly] public Vector2 inputVector;
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
    
    void Update()
    {
        int wallDirX = (movement.collisions.left) ? -1 : 1;
        wallSliding = false;

        float targetSpeed = AlterXSpeed();
        
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
    
    private float AlterXSpeed()
    {
        float speed;
        
        if (sprint)
        {
            xSpeed = sprintSpeed;
        }
        else if(crouch)
        {
            xSpeed = crouchSpeed;
        }
        else
        {
            xSpeed = moveSpeed;
        }
        
        if (movement.collisions.below)
        {
            lastSpeed = xSpeed;
            speed = xSpeed;
        }
        else
        {
            speed = lastSpeed;
        }

        return speed;
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