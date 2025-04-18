using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractAbility : MonoBehaviour
{
    public float interactRadius;
    public Transform interactOrigin;
    public LayerMask targetLayer;
    public float forceMagnitude;

    private Movement movement;

    private Vector2 forceDir;
    
    private void Start()
    {
        movement = GetComponent<Movement>();
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            if (Mathf.Sign(interactOrigin.localPosition.x) != movement.collisions.faceDir)
            {
                interactOrigin.localPosition = new Vector3(interactOrigin.localPosition.x * -1, interactOrigin.localPosition.y);
            }

            forceDir = new(movement.collisions.faceDir, 0);
            
        }
    }
    
    public void ApplyForceToObjects()
    {
        // Find all objects within the interact radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(interactOrigin.position, interactRadius, targetLayer);

        foreach (Collider2D collider in colliders)
        {
            Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
            
            if (rb != null)
            {
                // Apply force to the object
                rb.AddForce(forceDir.normalized * forceMagnitude, ForceMode2D.Impulse);
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        if (interactOrigin != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(interactOrigin.position, interactRadius);
        }
    }
    
}
