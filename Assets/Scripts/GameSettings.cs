using UnityEngine;
using TMPro;

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance;

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
        
        DontDestroyOnLoad(this);
    }
    
    public void DropDownSample(int index)
    {
        switch (index)
        {
            case 0:
                Debug.Log("res 1080");
                Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
                break;
            case 1:
                Debug.Log("res 1440");
                Screen.SetResolution(2560, 1440, FullScreenMode.FullScreenWindow);
                break;
            case 2:
                Debug.Log("res 4k");
                Screen.SetResolution(3840, 2160, FullScreenMode.FullScreenWindow);
                break;
            default:
                // Set res 1080
                Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
                break;
        }
    }
}
