using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CongklakMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject modeSelectionPanel;
    public Button vsAIButton;
    public Button vsFriendButton;
    public Button backButton;

    [Header("Friend Connection UI")]
    public GameObject friendConnectionPanel;
    public InputField codeInputField;
    public Button connectButton;
    public Button cancelButton;

    void Start()
    {
        modeSelectionPanel.SetActive(true);
        friendConnectionPanel.SetActive(false);

        vsAIButton.onClick.AddListener(StartVsAI);
        vsFriendButton.onClick.AddListener(ShowFriendConnection);
        backButton.onClick.AddListener(BackToMainMenu);
        connectButton.onClick.AddListener(ConnectToFriend);
        cancelButton.onClick.AddListener(CancelFriendConnection);
    }

    void StartVsAI()
    {
        CongklakGameManager.gameMode = GameMode.VsAI;
        SceneManager.LoadScene("CongklakGame");
    }

    void ShowFriendConnection()
    {
        modeSelectionPanel.SetActive(false);
        friendConnectionPanel.SetActive(true);
    }

    void ConnectToFriend()
    {
        string code = codeInputField.text;
        if (!string.IsNullOrEmpty(code))
        {
            CongklakGameManager.gameMode = GameMode.VsFriend;
            CongklakGameManager.roomCode = code;
            SceneManager.LoadScene("CongklakGame");
        }
    }

    void CancelFriendConnection()
    {
        friendConnectionPanel.SetActive(false);
        modeSelectionPanel.SetActive(true);
        codeInputField.text = "";
    }

    void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

public enum GameMode
{
    VsAI,
    VsFriend
}

public static class CongklakGameManager
{
    public static GameMode gameMode;
    public static string roomCode;
}