using System;
using UnityEngine;

public class BirdBehavior : MonoBehaviour
{
    public float catCheckRadius = 5;
    public float flySpeed = 2;
    public float flyUpForce;
    public float flyAwayForce = 5;
    public LayerMask playerLayer;
    public LayerMask groundLayer;
    public float groundDistanceCheck;


    private Vector2 velocity;
    private int flyDir;

    private BoxCollider2D collider;
    private Rigidbody2D rb;
    private Animator anim;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float min = -Mathf.Infinity;
        float max = Mathf.Infinity;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, catCheckRadius, playerLayer);

        if (hit)
        {
            if (hit.transform.CompareTag("Player"))
            {
                Transform player = hit.transform;

                flyDir = (int)Mathf.Sign(transform.position.x - player.transform.position.x);

                if (HitWallCheck(flyDir))
                {
                    flyDir *= -1;
                }

                if (Grounded())
                {
                    Vector2 force = new Vector2(flyAwayForce * flySpeed * flyDir, flyUpForce);
                    rb.AddForce(force, ForceMode2D.Force);
                }
            }
        }
        
        anim.SetBool("Grounded", Grounded()); 
    }

    private bool HitWallCheck(int currentDir)
    {
        Vector2 origin = new Vector2(collider.bounds.max.x * currentDir, collider.bounds.center.y);
        float rayLength = groundDistanceCheck;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.left * currentDir, rayLength, groundLayer);

        return hit.collider != null;
    }
    
    private bool Grounded()
    {
        Vector2 origin = new Vector2(collider.bounds.center.x, collider.bounds.min.y);
        float rayLength = groundDistanceCheck;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength, groundLayer);

        return hit.collider != null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, catCheckRadius);
    }
}
