using UnityEngine;
using UnityEngine.UI;

public class ClosedCanvasManager : MonoBehaviour
{
    [SerializeField] private Button profileButton;

    void Awake()
    {
        profileButton.onClick.AddListener(() => GameManager.instance.NextScene("Profile Screen"));
    }
    
}