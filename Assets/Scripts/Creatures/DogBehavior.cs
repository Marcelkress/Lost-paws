using UnityEngine;

public class DogBehavior : BehaviorStates
{
    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;

    public LayerMask playerLayer;
    public float distanceDelta;
    public float biteDist;
    public float waitTime = 1f;

    private Vector3 originalScale;
    private Vector3 velocity;
    private bool facingLeft;
    
    public float timePassed;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    public override void UpdateAnimations()
    {
        if (velocity.x < 0 && !facingLeft)
        {
            facingLeft = true;
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
        }
        else if (velocity.x > 0 && facingLeft)
        {
            facingLeft = false;
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
        }

        anim.SetBool("Moving", velocity.x != 0 ? true : false);
    }

    public override void Passive()
    {
        if (CheckForProximity())
        {
            currentState = State.Aggro;
        }
    }
    
    public bool CheckForProximity()
    {
        Collider2D hit = Physics2D.OverlapCircle(new(transform.position.x, transform.position.y), wakeUpRange, playerLayer);
        
        if (hit)
        {
            return true;
        }

        return false;
    }


    public override void Aggro()
    {
        if (player == null)
        {
            Collider2D hit = Physics2D.OverlapCircle(new(transform.position.x, transform.position.y), wakeUpRange, playerLayer);
            player = hit.transform;
        }

        Vector3 targetPos = new(player.position.x, transform.position.y);
        
        timePassed += Time.deltaTime;
        
        if (InBiteRange())
        {
            Debug.Log("BITE");
            timePassed = 0;
            velocity = Vector3.zero;
        }
        else if(timePassed > waitTime)
        {
            velocity = Vector3.MoveTowards(transform.position, targetPos, distanceDelta);
            rb.MovePosition(velocity);
        }
    }

    private bool InBiteRange()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * (facingLeft == true ? -1 : 1), biteDist, playerLayer);

        if (hit)
        {
            return true;
        }

        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new(transform.position.x, transform.position.y), wakeUpRange);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, new(transform.position.x + biteDist, transform.position.y));
    }
}
