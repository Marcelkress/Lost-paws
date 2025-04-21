using UnityEngine;
using UnityEngine.InputSystem;

public class Animations : MonoBehaviour
{
    private CatInput input;
    private Animator anim;
    private SpriteRenderer sprite;
    private Movement movement;
    private float lastPosX;
    private PlayerHealth ph;

    public float triggerJumpVelocityThreshold = 0.9f;
    public float moveXThreshold = 0.5f;
    
    void Start()
    {
        input = GetComponent<CatInput>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        movement = GetComponent<Movement>();
        ph = GetComponent<PlayerHealth>();
        
        ph.TakeDamageEvent.AddListener(TakeDamage);
    }
    
    void LateUpdate()
    {
        float newPosX = transform.position.x;
        float deltaX = Mathf.Abs(newPosX) - Mathf.Abs(lastPosX);
        anim.SetBool("Moving", Mathf.Abs(deltaX) > moveXThreshold ? true : false);
        lastPosX = transform.position.x;
        
        // Sprinting
        anim.SetBool("Sprinting", input.sprint);
        
        // Crouching
        anim.SetBool("Crouching", input.crouch);

        // Flipping sprite on input direction
        if (input.inputVector != Vector2.zero)
        {
            if (Mathf.Sign(input.inputVector.x) == -1)
            {
                sprite.flipX = true;
            }
            else
            {
                sprite.flipX = false;
            }
        }
        
        // Jumping 
        bool isInAir = !movement.collisions.below && !input.wallSliding;
        anim.SetBool("InAir", isInAir);
        
        

        if (Mathf.Abs(input.velocity.y) > 0.1)
        {
            float velocityY = Mathf.Clamp(input.velocity.y, -1, 1);

            float targetY = Mathf.Sign(velocityY);

            velocityY = Mathf.Lerp(velocityY, targetY, .05f);
            
            anim.SetFloat("JumpFloat", velocityY);
        }
        else
        {
            anim.SetFloat("JumpFloat", 0);
        }
        
        anim.SetBool("WallSliding", input.wallSliding);
        
    }
    
    public void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            anim.SetTrigger("Interact");
        }
    }

    void TakeDamage()
    {
        anim.SetTrigger("TakeDamage");
    }

}
