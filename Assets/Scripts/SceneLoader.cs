using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

public class SceneLoader : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("EXACT name of your Main Menu scene file. Case sensitive!")]
    public string mainMenuSceneName = "MainMenu";
    public void LoadMainMenu()
    {
        Debug.Log("[SceneLoader] 'LoadMainMenu' called from Button!");
        Screen.orientation = ScreenOrientation.Portrait;
        LoadScene(mainMenuSceneName);
    }

    public void LoadScene(string sceneName)
    {
        Debug.Log($"[SceneLoader] Attempting to load scene: '{sceneName}'");
        
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"[SceneLoader] ERROR: Scene '{sceneName}' not found! \n1. Check spelling (Case Sensitive).\n2. Add the scene to File -> Build Settings.");
        }
    }

    public void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log($"[SceneLoader] Reloading current scene: {currentScene.name}");
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"[SceneLoader] Loading next scene index: {nextSceneIndex}");
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("[SceneLoader] No more scenes! Returning to Main Menu.");
            LoadScene(mainMenuSceneName);
        }
    }

    public void QuitGame()
    {
        Debug.Log("[SceneLoader] Quitting Game...");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    void Update()
    {
        bool backButtonPressed = false;

        #if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            backButtonPressed = true;
        }
        #else
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            backButtonPressed = true;
        }
        #endif

        if (backButtonPressed)
        {
            HandleBackButton();
        }
    }

    private void HandleBackButton()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == mainMenuSceneName)
        {
            QuitGame();
        }
        else
        {
            Screen.orientation = ScreenOrientation.Portrait;
            LoadScene(mainMenuSceneName);
        }
    }
}