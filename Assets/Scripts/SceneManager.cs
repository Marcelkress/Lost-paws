using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public static SceneManager instance;
    private int currentSceneIndex;

    public float imgFadeTime;
    public Image sceneFadeImage;

    void Start()
    {
        currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        
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

    /// <summary>
    /// Loads the scene with the given build index
    /// </summary>
    /// <param name="index"></param>
    public void ChangeScene(int index)
    {
        currentSceneIndex = index;
        StartCoroutine(LoadScene(currentSceneIndex));
    }

    /// <summary>
    /// Loads the next scene in the build index
    /// </summary>
    public void ChangeScene()
    {
        currentSceneIndex++;
        StartCoroutine(LoadScene(currentSceneIndex));
    }

    private IEnumerator LoadScene(int sceneIndex)
    {
        // Fade screen to black
        StartCoroutine(FadeImage(false));
        
        // Wait for the fade time + 1 seconds
        yield return new WaitForSeconds(imgFadeTime + 1);

        // Load the new scene
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);

        // Wait until the scene is fully loaded
        while (asyncLoad.isDone == false)
        {
            yield return null;
        }
        
        // Fade back into game
        StartCoroutine(FadeImage(true));
    }

    IEnumerator FadeImage(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = imgFadeTime; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                sceneFadeImage.color = new Color(0, 0, 0, i);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= imgFadeTime; i += Time.deltaTime)
            {
                // set color with i as alpha
                sceneFadeImage.color = new Color(0, 0, 0, i);
                yield return null;
            }
        }
    }
}
