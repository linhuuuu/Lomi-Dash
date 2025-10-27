using UnityEngine;
using UnityEngine.UI;

public class canvasButttonsManager : MonoBehaviour
{
    [SerializeField] private Button almanac;
    [SerializeField] private Button store;
    [SerializeField] private Button profileButton;

    void Start()
    {
        almanac.onClick.AddListener(() => GameManager.instance.NextScene("Almanac Screen"));
        store.onClick.AddListener(() => GameManager.instance.NextScene("Shop Screen"));
        profileButton.onClick.AddListener(() => GameManager.instance.NextScene("Profile Screen"));
    }
}
