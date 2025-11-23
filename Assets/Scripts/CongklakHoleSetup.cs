using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CongklakHoleSetup : MonoBehaviour
{
    [Header("References")]
    public CongklakGameController gameController;
    public GameObject holeButtonPrefab;

    void Start()
    {
        SetupHoleButtons();
    }

    void SetupHoleButtons()
    {
        for (int i = 0; i < gameController.playerHolesParent.childCount; i++)
        {
            Transform hole = gameController.playerHolesParent.GetChild(i);
            Button button = hole.GetComponent<Button>();
            
            if (button != null)
            {
                int holeIndex = i;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => gameController.OnPlayerHoleClicked(holeIndex));
                
                button.interactable = true;
            }
        }
    }
}