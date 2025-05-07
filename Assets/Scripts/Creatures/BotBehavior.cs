using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Sirenix.OdinInspector;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class BotBehavior : CreatureBehavior
{
    [TitleGroup("Patrol settings")]
    public float waitTime;
    [TitleGroup("Patrol settings")]
    public float speed;
    [TitleGroup("Patrol settings")]
    public Vector3[] localWaypoints;
    [TitleGroup("Patrol settings")]
    [Range(0, 1)]public float easeAmount;
    public float maxViewAngle = 70;
    
    [TitleGroup("Laser Attack settings")]
    public float viewDistance;
    public GameObject projectile;
    public float laserDistance;
    public AudioClip laserBeamClip;
    public float projectileSpeed;
    public Transform instantiatePos;
    public float camShakeIntensity = 1f;
    public float camShakeTime = 0.1f;
    
    private Animator anim;
    private bool awake;
    private int fromWaypointIndex;
    private float percentBetweenWaypoints;
    private float nextMoveTime;
    private BoxCollider2D collider2D;
    private Collider2D hit;

    private Projectile projScript;
    
    private AudioSource audioSource;
    
    private Vector3[] globalWaypoints;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        collider2D = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();

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
    

    /// <summary>
    /// Method is called from the animation timeline in Unity
    /// </summary>
    public void Shoot()
    {
        if (hit == null)
            return;
        
        CameraShake.instance.Shake(camShakeIntensity, camShakeTime);
        
        // Instantiate prefab
        GameObject proj = Instantiate(projectile, instantiatePos.position, transform.rotation);
        projScript = proj.GetComponent<Projectile>();
        Vector2 target = hit.transform.position;
        projScript.Initialize(target, projectileSpeed);
    }

    float Ease(float x)
    {
        float a = easeAmount + 1;

        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }
    
    private bool PlayerInView()
    {
        if (CheckForProximity(viewDistance, ref hit))
        {
            if (hit.GetComponent<CatInput>().crouch)
            {
                return false;
            }
            
            /*
            // Raycast to check line of sight
            Vector2 rayDir = hit.transform.position - transform.position;
            rayDir.Normalize();
            RaycastHit2D rayHit = Physics2D.Raycast(instantiatePos.position, rayDir, viewDistance);
            if (!rayHit.transform.CompareTag("Player"))
            {
                return false;
            }
            */
            
            if (!facingLeft)
            {
                if (transform.position.x - hit.transform.position.x < 0)
                {
                    return true;
                }
            }
            else
            {
                if (transform.position.x - hit.transform.position.x > 0)
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    public void StartShootSound()
    {
        audioSource.PlayOneShot(laserBeamClip);
    }
    
    private Vector2 PatrolMovement()
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
