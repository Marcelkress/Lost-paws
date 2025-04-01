using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private float startPos;
    public GameObject cam;
    public float parallaxEffect;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position.x;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float distance = cam.transform.position.x * parallaxEffect; //  0 = move with cam \\ 1 = wont move
        
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }
}
