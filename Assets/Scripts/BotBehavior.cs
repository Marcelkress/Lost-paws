using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class BotBehavior : MonoBehaviour
{
    public float wakeUpRange;
    public float offsetToCenter;
    public LayerMask playerLayer;
    public float waitTime;
    public float speed;
    [UnityEngine.Range(0,2)] public float easeAmount;
    public float viewDistance;

    private Animator anim;
    private bool awake;
    private int fromWaypointIndex;
    private float percentBetweenWaypoints;
    private float nextMoveTime;
    private SpriteRenderer sprite;
    private BoxCollider2D collider2D;
    public Vector2 velocity;
    
    public Vector3[] localWaypoints;
    private Vector3[] globalWaypoints;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        collider2D = GetComponent<BoxCollider2D>();

        awake = false;

        
        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!awake)
        {
            CheckForProximity();
            return;
        }
        
        if (!PlayerInView())
        {
            velocity = PatrolMovement();
            transform.Translate(velocity);
        }
        
        anim.SetBool("InView", PlayerInView());
        
        if (velocity.x < 0)
        {
            sprite.flipX = true;
        }
        else if (velocity.x > 0)
        {
            sprite.flipX = false;
        }
        
        Debug.Log(PlayerInView());
    }

    private bool PlayerInView()
    {
        int castDir = (int)Mathf.Sign(velocity.x);

        RaycastHit2D hit = Physics2D.BoxCast(collider2D.bounds.center, collider2D.size, 0f, Vector2.right * castDir, viewDistance, playerLayer);
        
        return hit.collider != null;
    }
    
    Vector2 PatrolMovement()
    {
        if (Time.time < nextMoveTime)
        {
            return Vector2.zero;
        }
        
        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        
        float distanceBetweenWaypoints =
            Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * speed/distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);

        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);
        
        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex],
            easedPercentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;
            
            if (fromWaypointIndex >= globalWaypoints.Length - 1) 
            {
                fromWaypointIndex = 0; 
                System.Array.Reverse(globalWaypoints); 
            }
            
            nextMoveTime = Time.time + waitTime;
        }
        
        return newPos - transform.position;
    }

    private void CheckForProximity()
    {
        Collider2D hit = Physics2D.OverlapCircle(new(transform.position.x - offsetToCenter, transform.position.y), wakeUpRange, playerLayer);

        if (hit)
        {
            awake = true;
            anim.SetBool("Awake", awake);
        }
    }
    
    float Ease(float x)
    {
        float a = easeAmount + 1;

        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        Gizmos.DrawWireSphere(new(transform.position.x - offsetToCenter, transform.position.y), wakeUpRange);
        
        if (localWaypoints != null)
        {
            Gizmos.color = Color.green;
            float size = .3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos =
                    (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }

        Gizmos.color = Color.blue;
        int castDir = (int)Mathf.Sign(velocity.x); // Assuming the cast direction is based on the object's facing direction
        Vector3 castStart = transform.position;
        Vector3 castEnd = castStart + Vector3.right * castDir * viewDistance;
        Gizmos.DrawLine(castStart, castEnd);
    }
}
