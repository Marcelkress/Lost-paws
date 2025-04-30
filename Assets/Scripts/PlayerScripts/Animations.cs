using UnityEngine;
using UnityEngine.InputSystem;

public class Animations : MonoBehaviour
{
    private CatInput input;
    private Animator anim;
    private Movement movement;
    private PlayerHealth ph;

    public float triggerJumpVelocityThreshold = 0.9f;
    public float moveXThreshold = 0.5f;

    private Vector3 orgScale;
    
    void Start()
    {
        input = GetComponent<CatInput>();
        anim = GetComponent<Animator>();
        movement = GetComponent<Movement>();
        ph = GetComponent<PlayerHealth>();
        
        ph.TakeDamageEvent.AddListener(TakeDamage);
        
        orgScale = transform.localScale;
    }
    
    void LateUpdate()
    {
        anim.SetBool("Moving", input.inputVector != Vector2.zero ? true : false);
        
        // Sprinting
        anim.SetBool("Sprinting", input.sprint);
        
        // Crouching
        anim.SetBool("Crouching", input.crouch);

        // Flipping sprite on input direction
        if (input.inputVector != Vector2.zero)
        {
            if (Mathf.Sign(input.inputVector.x) == -1)
            {
                //sprite.flipX = true;
                transform.localScale = new(-orgScale.x, transform.localScale.y);
            }
            else
            {
                //sprite.flipX = false;
                transform.localScale = orgScale;
            }
        }
        
        // Jumping 
        bool isInAir = !movement.collisions.below && !input.wallSliding && Mathf.Abs(input.velocity.y)  > .9f;
        anim.SetBool("InAir", isInAir);

        if (isInAir)
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

    void TakeDamage()
    {
        anim.SetTrigger("TakeDamage");
    }

}
