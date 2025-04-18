using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float hitRadius, speed;
    private Vector2 direction;

    public void Initialize(Vector2 targetPosition)
    {
        direction = (Vector2)transform.position - targetPosition;
        direction.Normalize();
        
        transform.Rotate(direction);
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newPos = direction * Time.deltaTime * speed;
        
        transform.Translate(newPos);
    }
}
