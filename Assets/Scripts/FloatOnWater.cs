using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FloatOnWater : MonoBehaviour
{
    public float upForce = 1f;
    public float damping = 0.5f;
    public LayerMask obstacleLayer;
    
    private float desiredYPosition;
    private Rigidbody2D rb;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            desiredYPosition = transform.position.y;
            int layerIndex = (int)Mathf.Log(obstacleLayer.value, 2);
            gameObject.layer = layerIndex;
        }
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Water"))
        {
            return;
        }
        
        float floatScaleFactor = desiredYPosition - transform.position.y;

        float yForce = upForce * floatScaleFactor - damping * rb.linearVelocity.y;
        
        Vector2 force = new(0, yForce);
        
        rb.AddForce(force, ForceMode2D.Impulse);
        //rb.linearVelocity = new(0,rb.linearVelocity.y);
    }
}