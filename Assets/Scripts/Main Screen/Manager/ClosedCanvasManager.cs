using UnityEngine;
using UnityEngine.UI;

public class ClosedCanvasManager : MonoBehaviour
{
    [SerializeField] private Button profileButton;
    [SerializeField] private Button ucgButton;

    void Awake()
    {
        profileButton.onClick.AddListener(() => GameManager.instance.NextScene("Profile Screen"));


    }
    
}