using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private bool isPaused;
    public static GameManager instance;
    public GameObject player;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void TogglePauseTime()
    {
        if (!isPaused)
        {
            isPaused = true;
            Time.timeScale = 0;
            // Switch action map
        }
        else
        {
            isPaused = false;
            Time.timeScale = 1;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.instance.ChangeScene();
        }
    }
    
}
