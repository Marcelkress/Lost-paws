using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class InteractAbility : MonoBehaviour
{
    public float interactRadius;
    public Transform interactOrigin;
    public LayerMask targetLayer;
    public float forceMagnitude;

    private Movement movement;
    private GameObject heldObj;

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
    
    /// <summary>
    /// Called in animation timeline
    /// </summary>
    public void InteractWithObjects()
    {
        if (heldObj != null)
        {
            // Drop Object
            heldObj.GetComponent<IInteractable>().Interact(interactOrigin);
            heldObj.transform.parent = null;
            heldObj = null;
            return;
        }
        
        // Find all objects within the interact radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(interactOrigin.position, interactRadius, targetLayer);
        
        foreach (Collider2D collider in colliders)
        {
            if (heldObj == null && collider.transform.GetComponent<IInteractable>() != null)
            {
                heldObj = collider.gameObject;
                heldObj.GetComponent<IInteractable>().Interact(transform);
            }
            else
            {
                // Push object
                Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();

                if (rb != null)
                { 
                    // Apply force to the object
                    rb.AddForce(forceDir.normalized * forceMagnitude, ForceMode2D.Impulse);
                }
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
