using UnityEngine;
using UnityEngine.UI;

public class DebugHoleClicks : MonoBehaviour
{
    void Start()
    {
        Transform playerHoles = GameObject.Find("PlayerHoles").transform;
        
        for (int i = 0; i < playerHoles.childCount; i++)
        {
            Button button = playerHoles.GetChild(i).GetComponent<Button>();
            if (button != null)
            {
                int index = i;
                button.onClick.AddListener(() => Debug.Log($"Hole {index} clicked!"));
            }
        }
    }
}