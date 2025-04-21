using System;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float hitRadius;
    public int damage; 
    public float lifeTime;
    public LayerMask obstacles;

    private float timePassed;
    private float speed;
    private Vector2 direction;

    public void Initialize(Vector2 targetPosition, float speed)
    {
        this.speed = speed;

        direction = (Vector2)transform.position - targetPosition; 
        direction.Normalize();

        transform.right = -direction;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newPos = new Vector2(1f, 0f) * Time.deltaTime * speed;
        
        transform.Translate(newPos);

        timePassed += Time.deltaTime;
        if (timePassed > lifeTime)
        {
            Destroy(this.GameObject());
        }
        
        CheckHit();
    }

    private void CheckHit()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, hitRadius);

        if (hit != null)
        {
            if(hit.CompareTag("Player"))
            {
                hit.transform.GetComponent<IHealth>().TakeDamage(damage, false);
                Destroy(this.GameObject());

            }
            else
            {
                Destroy(this.GameObject());
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(new(transform.position.x, transform.position.y), hitRadius);
    }


}
