using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffsManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button button;

    void Start()
    {
        if (GameManager.instance.state == GameManager.gameState.beforeDay)
            return; 
        
        panel.SetActive(false);
        button.onClick.AddListener(() => TogglePanel());
    }

    private void TogglePanel()
    {
        panel.SetActive(!panel.activeSelf);
    }
}
