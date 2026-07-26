using UnityEngine;

// Button-friendly proxy for the LevelLoader singleton.
// Buttons can't hold a reliable reference to a DontDestroyOnLoad singleton across scenes,
// so put this component on a scene object and wire the Button's OnClick to these methods -
// they forward to LevelLoader.Instance at click time.
public class LevelLoaderButton : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        if (!Ready()) return;
        LevelLoader.Instance.LoadLevel(sceneName);
    }

    public void LoadSceneByIndex(int buildIndex)
    {
        if (!Ready()) return;
        LevelLoader.Instance.LoadLevel(buildIndex);
    }

    public void LoadNextScene()
    {
        if (!Ready()) return;
        LevelLoader.Instance.LoadNext();
    }

    public void ReloadScene()
    {
        if (!Ready()) return;
        LevelLoader.Instance.ReloadLevel();
    }

    private bool Ready()
    {
        if (LevelLoader.Instance == null)
        {
            Debug.LogError("[LevelLoaderButton] No LevelLoader.Instance found. Ensure a LevelLoader " +
                           "exists in the first-loaded scene (it is DontDestroyOnLoad).", this);
            return false;
        }
        return true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
