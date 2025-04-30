using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class InteractAbility : MonoBehaviour
{
    public float interactRadius;
    public Transform interactOrigin, mouthPosition;
    public LayerMask targetLayer;
    public float forceMagnitude;

    private Animator anim;
    private Movement movement;
    private GameObject heldObj;

    private Vector2 forceDir;
    
    private void Start()
    {
        movement = GetComponent<Movement>();
        anim = GetComponent<Animator>();
    }
    
    public void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            forceDir = new(movement.collisions.faceDir, 0);
            InteractWithObjects();
        }
    }

    private Collider2D[] colliders;
    
    private void InteractWithObjects()
    {
        if (heldObj != null)
        {
            // Put down object we are holding
            anim.SetTrigger("Pickup");
            return;
        }
        
        // Find all objects within the interact radius
        colliders = Physics2D.OverlapCircleAll(interactOrigin.position, interactRadius, targetLayer);
        
        foreach (Collider2D collider in colliders)
        {
            if (heldObj == null && collider.transform.GetComponent<IInteractable>() != null)
            {
                anim.SetTrigger("Pickup");
            }
            else
            {
                anim.SetTrigger("Push");
            }
        }
    }

    /// <summary>
    /// Called in animation timeline
    /// </summary>
    public void PickupObject()
    {
        if (heldObj != null)
        {
            // Drop Object
            heldObj.GetComponent<IInteractable>().Interact(mouthPosition);
            heldObj.transform.parent = null;
            heldObj = null;
            return;
        }

        colliders = Physics2D.OverlapCircleAll(interactOrigin.position, interactRadius, targetLayer);

        foreach (Collider2D collider in colliders)
        {
            if (heldObj == null && collider.transform.GetComponent<IInteractable>() != null)
            {
                if(collider.transform.CompareTag("PlatformKeyPickup"))
                {
                    heldObj = collider.gameObject;
                    heldObj.GetComponent<IInteractable>().Interact(mouthPosition);
                }
                else if(collider.transform.CompareTag("UnlockObj"))
                {
                    collider.transform.GetComponent<IInteractable>().Interact(this.transform);
                }
            }
        }
    }

    /// <summary>
    /// Called in animation timeline
    /// </summary>
    public void PushObject()
    {
        colliders = Physics2D.OverlapCircleAll(interactOrigin.position, interactRadius, targetLayer);
        foreach (Collider2D collider in colliders)
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
    
    private void OnDrawGizmos()
    {
        if (interactOrigin != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(interactOrigin.position, interactRadius);
        }
    }
    
}
