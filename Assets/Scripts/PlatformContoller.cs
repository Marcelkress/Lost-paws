using UnityEngine;

public class PlatformContoller : RaycastController
{
    public Vector3 move;
    
    public override void Start()
    {
        base.Start();
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 veloctiy = move * Time.deltaTime;
        transform.Translate(veloctiy);
    }

    void MovePassengers(Vector3 velocity)
    {
        float direcitonX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);
        
        // Vertically moving platform
        if (velocity.y != 0)
        {
            
        }
    }
}
