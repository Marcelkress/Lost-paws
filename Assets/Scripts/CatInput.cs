using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Movement))]
public class CatInput : MonoBehaviour
{
    [Header("Input settings")] 
    
    public float jumpHeight = 4f;
    public float timeToJumpMax = 0.4f;
    public float moveSpeed;
    public float sprintSpeed;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = 0.1f;
    
    private float gravity;
    private float jumpVelocity;
    public Vector3 velocity;
    private Movement movement;
    
    [HideInInspector]
    public Vector2 inputVector;
    public bool jump;
    public bool sprint;

    private float velocityXSmoothing;

    void Start()
    {
        movement = GetComponent<Movement>();
 
        gravity = -(2 * jumpHeight / Mathf.Pow(timeToJumpMax, 2));
        jumpVelocity = Mathf.Abs(gravity * timeToJumpMax);
    }

    void Update()
    {
        if (movement.collisions.above || movement.collisions.below)
        {
            velocity.y = 0;
        }
        if (jump && movement.collisions.below)
        {
            velocity.y = jumpVelocity;
        }
        
        // Movement stuff
        float targetVelocityX = inputVector.x * (sprint ? sprintSpeed : moveSpeed);
        
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing,
            (movement.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne));
        velocity.y += gravity * Time.deltaTime;
        
        movement.Move(velocity * Time.deltaTime);
    }
    
    public void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }
    public void OnJump(InputValue value)
    {
        jump = value.isPressed;
    }
    public void OnSprint(InputValue value)
    {
        sprint = value.isPressed;
    }
}