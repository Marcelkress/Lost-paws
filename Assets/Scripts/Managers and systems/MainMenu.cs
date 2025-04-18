using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public float animDuration = 0.5f;
    [Header("Panel menus")]
    public RectTransform mainPanel;
    public RectTransform settingsPanel;
    public RectTransform controlsPanel;

    [Header("Buttons for selection")] 
    public GameObject backButtonControls;
    public GameObject backButtonSettings;
    public GameObject startButton;

    [Header("Positions the menus move between")]
    public RectTransform mainPos;
    public RectTransform bottomPos;
    public RectTransform rightPos;

    private bool gameStarted;

    void Start()
    {
        gameStarted = false;
    }
    
    public void StartGame()
    {
        if (gameStarted)
            return;
        
        gameStarted = true;
        SceneManager.instance.ChangeScene();
    }

    public void OpenSettings()
    {
        Debug.Log("Settings open");
        mainPanel.DOAnchorPos(bottomPos.anchoredPosition, animDuration, true).SetUpdate(true);
        settingsPanel.DOAnchorPos(mainPos.anchoredPosition, animDuration, true).SetUpdate(true);
        
        //Clear
        EventSystem.current.SetSelectedGameObject(null);
        //Reassign
        EventSystem.current.SetSelectedGameObject(backButtonSettings);
    }

    public void CloseSettings()
    {
        mainPanel.DOAnchorPos(mainPos.anchoredPosition, animDuration, true).SetUpdate(true);
        settingsPanel.DOAnchorPos(rightPos.anchoredPosition, animDuration, true).SetUpdate(true);
        
        //Clear
        EventSystem.current.SetSelectedGameObject(null);
        //Reassign
        EventSystem.current.SetSelectedGameObject(startButton);
    }

    public void OpenControls()
    {
        controlsPanel.DOAnchorPos(mainPos.anchoredPosition, animDuration, true).SetUpdate(true);
        mainPanel.DOAnchorPos(bottomPos.anchoredPosition, animDuration, true).SetUpdate(true);
        
        //Clear
        EventSystem.current.SetSelectedGameObject(null);
        //Reassign
        EventSystem.current.SetSelectedGameObject(backButtonControls);
    }

    public void CloseControls()
    {
        mainPanel.DOAnchorPos(mainPos.anchoredPosition, animDuration, true).SetUpdate(true);
        controlsPanel.DOAnchorPos(rightPos.anchoredPosition, animDuration, true).SetUpdate(true);
        
        //Clear
        EventSystem.current.SetSelectedGameObject(null);
        //Reassign
        EventSystem.current.SetSelectedGameObject(startButton);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME");
        Application.Quit();
    }
}