using System.Net;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Movement))]
public class CatInput : MonoBehaviour
{
    [Header("Jump settings")] 
    public float maxJumpHeight = 4f;
    public float minJumpHeight = 1f;
    public float timeToJumpMax = 0.4f;

    [Header("Movement settings")] 
    public float moveSpeed;
    public float sprintSpeed;
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
    
    [HideInInspector] public Vector3 velocity;
    private Movement movement;
    private BoxCollider2D collider;
    
    [HideInInspector] public Vector2 inputVector;
    [HideInInspector] public bool jumpTrigger;
    [HideInInspector] public bool jumpRelease;
    [HideInInspector] public bool sprint;

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
        
        float targetVelocityX = inputVector.x * (sprint ? sprintSpeed : moveSpeed);
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
            if (movement.collisions.below)
            {
                velocity.y = maxJumpVelocity;
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
        jumpTrigger = !jumpTrigger;
    }

    private void JumpReleased(InputAction.CallbackContext context)
    {
        jumpRelease = true;
    }
    
    public void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }
    
    public void OnJump(InputValue value)
    {
        jumpTrigger = value.isPressed;
    }
    
    public void OnSprint(InputValue value)
    {
        sprint = value.isPressed;
    }
}