using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public float animDuration = 0.5f;
    public PlayerInput playerInput;
    private Image fadeOverlay;
    private InputAction pauseAction;
    private bool isPaused;
    
    [Header("Panel menus")]
    public RectTransform pausePanel;
    public RectTransform settingsPanel;

    [Header("Buttons for selection")] 
    public GameObject backButton;
    public GameObject resumeButton;

    [Header("Positions the menus move between")]
    public RectTransform mainPos;
    public RectTransform bottomPos;
    public RectTransform rightPos;

    void Start()
    {
        pauseAction = playerInput.actions["Pause"];
        pauseAction.performed += Pause;

        fadeOverlay = GetComponent<Image>();
        
        fadeOverlay.enabled = false;
        isPaused = false;
    }

    private void Pause(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if (!isPaused)
        {
            isPaused = true;
            fadeOverlay.enabled = true;
            GameManager.instance.TogglePauseTime();
            pausePanel.DOAnchorPos(mainPos.anchoredPosition, animDuration, true).SetUpdate(true);
            //Clear
            EventSystem.current.SetSelectedGameObject(null);
            //Reassign
            EventSystem.current.SetSelectedGameObject(resumeButton);
        }
        else
        {
            isPaused = false;
            fadeOverlay.enabled = false;
            GameManager.instance.TogglePauseTime();
            pausePanel.DOAnchorPos(bottomPos.anchoredPosition, animDuration, true).SetUpdate(true);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OpenSettings()
    {
        Debug.Log("Settings open");
        pausePanel.DOAnchorPos(bottomPos.anchoredPosition, animDuration, true).SetUpdate(true);
        settingsPanel.DOAnchorPos(mainPos.anchoredPosition, animDuration, true).SetUpdate(true);
        
        //Clear
        EventSystem.current.SetSelectedGameObject(null);
        //Reassign
        EventSystem.current.SetSelectedGameObject(backButton);
    }

    public void CloseSettings()
    {
        pausePanel.DOAnchorPos(mainPos.anchoredPosition, animDuration, true).SetUpdate(true);
        settingsPanel.DOAnchorPos(rightPos.anchoredPosition, animDuration, true).SetUpdate(true);
        
        //Clear
        EventSystem.current.SetSelectedGameObject(null);
        //Reassign
        EventSystem.current.SetSelectedGameObject(resumeButton);
    }

    private bool pressed = false;

    public void MainMenu()
    {
        if (!pressed)
        {
            SceneManager.instance.ChangeScene(0);
            pressed = true;
        }
    }
}
