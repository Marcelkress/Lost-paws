using System.Collections;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class BotBehavior : CreatureBehavior
{
    [Header("Settings")]
    public float waitTime;
    public float speed;
    public Vector3[] localWaypoints;
    [Range(0, 1)]public float easeAmount;
    
    
    [Header("Laser attack")]
    public float viewDistance;
    public GameObject laserBeam;
    public float laserActiveTime = 0.5f;
    public float laserDistance;
    public int laserDamage;
    public AudioClip laserBeamClip;

    private Animator anim;
    private bool awake;
    private int fromWaypointIndex;
    private float percentBetweenWaypoints;
    private float nextMoveTime;
    private BoxCollider2D collider2D;
    private Collider2D hit;
    
    private AudioSource audioSource;
    
    private Vector3[] globalWaypoints;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        collider2D = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();

        awake = false;
        originalScale = transform.localScale;
        
        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        
        if (!awake)
        {
            return;
        }

        if (!PlayerInView())
        {
            currentState = State.Patrol;
        }
        else
        {
            currentState = State.Aggro;
        }
    }

    public override void Patrol()
    {
        velocity = PatrolMovement();
        transform.Translate(velocity);
    }

    public override void Aggro()
    {
        anim.SetBool("InView", PlayerInView());
    }

    public override void Passive()
    {
        if (CheckForProximity(detectRange, ref hit))
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

    /// <summary>
    /// Method is called from the animation timeline in Unity
    /// </summary>
    public void Shoot()
    {
        StartCoroutine(ShootLaser());
        
        RaycastHit2D hit = Physics2D.CircleCast(collider2D.bounds.center, 2f, Vector2.right * (facingLeft == true ? -1 : 1), laserDistance, playerLayer);

        if (hit.collider != null)
        {
            hit.transform.GetComponent<IHealth>().TakeDamage(laserDamage);
        }
    }

    private bool PlayerInView()
    {
        RaycastHit2D hit = Physics2D.BoxCast(collider2D.bounds.center, collider2D.bounds.size, 0f, Vector2.right * (facingLeft == true ? -1 : 1), viewDistance, playerLayer);
        
        return hit.collider != null;
    }
    
    private IEnumerator ShootLaser()
    {
        laserBeam.SetActive(true);
        

        yield return new WaitForSeconds(laserActiveTime);
        
        laserBeam.SetActive(false);
    }

    public void StartShootSound()
    {
        audioSource.PlayOneShot(laserBeamClip);
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
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        Gizmos.DrawWireSphere(new(transform.position.x, transform.position.y), detectRange);
        
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
        Vector3 castEnd = castStart + Vector3.right * castDir * laserDistance;
        Gizmos.DrawLine(castStart, castEnd);
    }
}
