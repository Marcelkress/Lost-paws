using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    [Header("Dust land")]
    public float dustRaycastOffset = 0.5f;
    public float dustRaycastLength = .5f;
    public GameObject dustLandEffect;
    private bool wasInAir;

    [Header("Dust Sprint")] 
    public GameObject sprintDust;

    public float spawnOffset = -0.1f;
    private bool wasSprinting;
    
    
    private Movement movement;
    private CatInput input;


    void Start()
    {
        wasInAir = false;
        movement = GetComponent<Movement>();
        input = GetComponent<CatInput>();
    }
    
    void LateUpdate()
    {
        DustLandEffect();
        SprintStartDust();
    }

    private void SprintStartDust()
    {
        if (input.sprint && !wasSprinting)
        {
            GameObject dust = Instantiate(sprintDust, new(transform.position.x, transform.position.y - spawnOffset), sprintDust.transform.rotation);
            
            //dust.transform.SetPositionAndRotation(transform.position, quaternion.identity);

            if (input.inputVector.x != 1)
            {
                Vector3 scale = new(dust.transform.localScale.x, dust.transform.localScale.y * -1);
                dust.transform.localScale = scale;
            }
        }
        wasSprinting = input.sprint;
    }

    private void DustLandEffect()
    {
        bool isInAir = !movement.collisions.below && !input.wallSliding;
        
        // Detect landing
        if (wasInAir && !isInAir)
        {
            RaycastHit2D hit = Physics2D.Raycast( new(transform.position.x,transform.position.y - dustRaycastOffset), Vector2.down, dustRaycastLength);

            if (hit)
            {
                if (hit.transform.CompareTag("Dirt"))
                {
                    Instantiate(dustLandEffect, transform.position, quaternion.identity);
                }
            }
        }
        wasInAir = isInAir;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new(transform.position.x,transform.position.y - dustRaycastOffset), new(transform.position.x,transform.position.y - dustRaycastLength));
    }
}
