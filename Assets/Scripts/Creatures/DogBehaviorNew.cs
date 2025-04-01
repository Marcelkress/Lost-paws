using UnityEngine;

public class DogBehaviorNew : CreatureBehavior
{
    public float biteRange;
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
        
        //Debug.Log(velocity);
        
    }

    public override void Aggro()
    {
        // If player is within bite range, stop running and bite
        if (CheckForProximity(biteRange, ref hit))
        {
            biting = true;
            timePassed = 0;
            Debug.Log("Bite");
            
            // Stop running
            velocity = Vector3.zero;

            // Play bite animation
        }
        
        timePassed += Time.deltaTime;
        if (timePassed > biteTime)
        {
            biting = false;
        }
        
        // If player is still in range run towards them
        if (CheckForProximity(detectRange, ref hit) && !biting)
        {
            timePassed = 0;
            Vector3 targetPos = new (player.position.x, transform.position.y);
            Vector3 dir = (targetPos - transform.position).normalized;
            velocity = dir * speed * Time.deltaTime;
            
            //Debug.Log(velocity);
            
            // If a wall is detected, stop running and bark
            RaycastHit2D wallHit = Physics2D.Raycast(transform.position, dir, biteRange, obstacleLayer);
            
            if (wallHit.collider != null)
            {
                velocity = Vector3.zero;
                anim.SetBool("Barking", true);
            }
            else
            {
                anim.SetBool("Barking", false);
            }

            // If the x distance to the player is within biteRange but y is not, stop and bark.
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
