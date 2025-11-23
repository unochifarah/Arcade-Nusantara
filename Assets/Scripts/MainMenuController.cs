using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button congklakButton;
    public Button egrangButton;
    public Button settingsButton;

    void Start()
    {
        congklakButton.onClick.AddListener(PlayCongklak);
        egrangButton.onClick.AddListener(PlayEgrang);
        settingsButton.onClick.AddListener(OpenSettings);
    }

    void PlayCongklak()
    {
        SceneManager.LoadScene("Congklak");
    }

    void PlayEgrang()
    {
        SceneManager.LoadScene("Egrang");
    }

    void OpenSettings()
    {
        Debug.Log("Settings button clicked");
    }
}