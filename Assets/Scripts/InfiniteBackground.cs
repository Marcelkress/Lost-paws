using System;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    private float startPos;
    private SpriteRenderer spriteRenderer;
    
    public GameObject cam;
    public float parallaxEffect;

    public Sprite[] sprites;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPos = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = cam.transform.position.x * parallaxEffect; //  0 = move with cam \\ 1 = won't move
        
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }
}

