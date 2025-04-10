using UnityEngine;
using UnityEngine.InputSystem;

public class Miaw : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] miawSounds;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    public void OnMiaw(InputValue value)
    {
        if (value.isPressed)
        {
            int randVal = Random.Range(0, miawSounds.Length);
            
            audioSource.PlayOneShot(miawSounds[randVal]);
        }
    }
}
