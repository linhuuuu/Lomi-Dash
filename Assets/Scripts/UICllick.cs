using UnityEngine;
using UnityEngine.UI;


public class UIClick : MonoBehaviour
{
    private Button btn;

    void Awake()
    {
        btn = GetComponent<Button>();
    }
    void Start()
    {
        if (AudioManager.instance != null)
            btn.onClick.AddListener(() => AudioManager.instance.PlayUI(UI.CLICK));
    }
}