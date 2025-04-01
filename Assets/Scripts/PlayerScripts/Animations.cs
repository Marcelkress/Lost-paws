using UnityEngine;
public class Animations : MonoBehaviour
{
    private CatInput input;
    private Animator anim;
    private SpriteRenderer sprite;
    private Movement movement;
    private BoxCollider2D collider;
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
        collider = GetComponent<BoxCollider2D>();
        ph = GetComponent<PlayerHealth>();
        
        ph.TakeDamageEvent.AddListener(TakeDamage);
    }
    
    void LateUpdate()
    {
        // Moving only if we moved since last frame to avoid the expanding collider going through walls
        float newPosX = transform.position.x;
        float deltaX = Mathf.Abs(newPosX) - Mathf.Abs(lastPosX);
        anim.SetBool("Moving", Mathf.Abs(deltaX) > moveXThreshold ? true : false);
        lastPosX = transform.position.x;
        
        // Sprinting
        anim.SetBool("Sprinting", input.sprint);

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
        
        float velocityY = Mathf.Sign(input.velocity.y); 
        anim.SetFloat("JumpFloat", velocityY);
        
        anim.SetBool("WallSliding", input.wallSliding);
    }

    void TakeDamage()
    {
        anim.SetTrigger("TakeDamage");
    }

}
