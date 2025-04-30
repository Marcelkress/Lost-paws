using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlatformKeyObject : MonoBehaviour
{
    public PlatformContoller targetPlatform;
    private SpriteRenderer spriteRenderer;

    public Sprite upSprite, downSprite;
    
    void Start()
    {
        targetPlatform.LockPlatform(true);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlatformKey") || other.CompareTag("PlatformKeyPickup"))
        {
            targetPlatform.LockPlatform(false);
            spriteRenderer.sprite = downSprite;
            other.GetComponent<Light2D>().enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlatformKey") || other.CompareTag("PlatformKeyPickup"))
        {
            targetPlatform.LockPlatform(true);
            spriteRenderer.sprite = upSprite;
            other.GetComponent<Light2D>().enabled = false;
        }
    }
}
