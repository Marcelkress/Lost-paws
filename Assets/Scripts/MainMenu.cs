using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public float animDuration = 0.5f;
    [Header("Panel menus")]
    public RectTransform mainPanel;
    public RectTransform settingsPanel;

    [Header("Buttons for selection")] 
    public GameObject backButton;
    public GameObject startButton;

    [Header("Positions the menus move between")]
    public RectTransform mainPos;
    public RectTransform bottomPos;
    public RectTransform rightPos;
    
    public void StartGame()
    {
        SceneManager.instance.ChangeScene();
        
    }

    public void OpenSettings()
    {
        Debug.Log("Settings open");
        mainPanel.DOAnchorPos(bottomPos.anchoredPosition, animDuration, true);
        settingsPanel.DOAnchorPos(mainPos.anchoredPosition, animDuration, true);
        
        //Clear
        EventSystem.current.SetSelectedGameObject(null);
        //Reassign
        EventSystem.current.SetSelectedGameObject(backButton);
    }

    public void CloseSettings()
    {
        mainPanel.DOAnchorPos(mainPos.anchoredPosition, animDuration, true);
        settingsPanel.DOAnchorPos(rightPos.anchoredPosition, animDuration, true);
        
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