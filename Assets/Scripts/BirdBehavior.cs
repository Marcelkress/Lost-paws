using System;
using System.Collections;
using UnityEngine;

public class BirdBehavior : MonoBehaviour
{
    public float catCheckRadius = 5;
    public LayerMask playerLayer;
    public LayerMask groundLayer;
    public float groundDistanceCheck;
    
    public float flyHeight = 5;
    public float flyDistance;
    public float flySpeed = 5;
    public float flyDuration = 2;
    
    private BoxCollider2D collider2d;
    //private Rigidbody2D rb;
    private Animator anim;
    private bool isFlying;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider2d = GetComponent<BoxCollider2D>();
        //rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    
    void Update()
    {
        if (!isFlying)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, catCheckRadius, playerLayer);

            if (hit)
            {
                Transform player = hit.transform;
                float flyDir = Mathf.Sign(transform.position.x - player.position.x);
                StartCoroutine(FlyAway(flyDir));
            }
        }
    }

    private IEnumerator FlyAway(float flyDir)
    {
        yield return new WaitForSeconds(flyDuration);
    }
    private bool HitWallCheck(int currentDir)
    {
        Vector2 origin = new Vector2(collider2d.bounds.max.x * currentDir, collider2d.bounds.center.y);
        float rayLength = groundDistanceCheck;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.left * currentDir, rayLength, groundLayer);

        return hit.collider != null;
    }
    
    private bool Grounded()
    {
        Vector2 origin = new Vector2(collider2d.bounds.center.x, collider2d.bounds.min.y);
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
