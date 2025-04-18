using UnityEngine;

public class DogBehaviorNew : CreatureBehavior
{
    public float biteRange;
    public float barkRange;
    public float speed;
    public LayerMask obstacleLayer;
    public float biteTime = 1f;

    private float timePassed;
    private Transform player;
    private Collider2D hit;
    private Vector3 velocityRef;
    private Rigidbody2D rb;
    private Animator anim;

    private bool biting;
    private float percentToPlayer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        
        anim.SetBool("Moving", (Mathf.Abs(velocity.x) != 0 ? true : false));
    }

    /// <summary>
    /// Method called from animation event
    /// </summary>
    public void Bite()
    {
        hit.transform.GetComponent<IHealth>().TakeDamage(1, false);
        Debug.Log("bite!");
    }

    public override void Aggro()
    {
        Vector3 targetPos = new (player.position.x, transform.position.y);
        Vector3 dir = (targetPos - transform.position).normalized;
        
        // If player is within bite range, stop running and bite
        if (CheckForProximity(biteRange, ref hit))
        {
            biting = true;
            timePassed = 0;
            
            // Stop running
            velocity = Vector3.zero;

            // Play bite animation
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, biteRange, playerLayer);
            
            if (hit.collider != null)
            {
                anim.SetBool("Biting", true);
            }
            else
            {
                anim.SetBool("Biting", false);
            }
        }
        
        timePassed += Time.deltaTime;
        if (timePassed > biteTime)
        {
            biting = false;
        }
        
        // If player is still in range && we're not biting, run towards the player
        if (CheckForProximity(detectRange, ref hit) && !biting)
        {
            timePassed = 0;
            
            velocity = dir * speed * Time.deltaTime;
            
            //Debug.Log(velocity);
            
            // If a wall is detected, stop running and bark
            RaycastHit2D wallHit = Physics2D.Raycast(transform.position, dir, barkRange, obstacleLayer);
            if (wallHit.collider != null)
            {
                velocity = Vector3.zero;
                anim.SetBool("Barking", true);
            }
            // If the x distance to the player is within biteRange but y is not, stop and bark.
            else if (Vector2.Distance(transform.position, targetPos) < barkRange &&
                Vector2.Distance(transform.position, player.position) > barkRange)
            {
                velocity = Vector3.zero;
                anim.SetBool("Barking", true);
            }
            else
            {
                anim.SetBool("Barking", false);
            }
            
            
        }
        
        // If the player is not within detection range then go back to passive state
        else
        {
            currentState = State.Passive;
        }
        
        transform.Translate(velocity);
    }
    
    public override void Passive()
    {
        if (CheckForProximity(detectRange, ref hit))
        {
            currentState = State.Aggro;
            
            if (player == null)
            {
                player = hit.transform;
            }
        }
    }
}
