using UnityEngine;
public class Animations : MonoBehaviour
{
    private CatInput input;
    private Animator anim;
    private SpriteRenderer sprite;
    private Movement movement;

    public float triggerJumpVelocityThreshold = 0.9f;
    
    void Start()
    {
        input = GetComponent<CatInput>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        movement = GetComponent<Movement>();
    }
    
    void LateUpdate()
    {
        // Moving and sprinting
        anim.SetBool("Moving", input.inputVector.x != 0 ? true : false);
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
        if (Mathf.Abs(input.velocity.y) > triggerJumpVelocityThreshold && !movement.collisions.below)
        {
            anim.SetBool("InAir", true);
            
            float velocityY = Mathf.Sign(input.velocity.y);
            
            anim.SetFloat("JumpFloat", velocityY);
        }
        else
        {
            anim.SetBool("InAir", false);
        }
    }
}
