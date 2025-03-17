using UnityEngine;
public class Animations : MonoBehaviour
{
    private CatInput input;
    private Animator anim;
    private SpriteRenderer sprite;
    private Movement movement;
    
    void Start()
    {
        input = GetComponent<CatInput>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        movement = GetComponent<Movement>();
    }
    
    void Update()
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
        
        if (!movement.collisions.below)
        {
            if (Mathf.Sign(input.velocity.y) == 1)
            {
                // Set jumping float
            }
        }
    }
}
