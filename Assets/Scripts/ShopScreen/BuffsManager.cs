using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffsManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button button;
    [SerializeField] private Button buffChoicePanelButton;
    [SerializeField] private GameObject buffChoicePanel;
    [SerializeField] private List<BuffDrag> buffDrags;

    void Start()
    {
        buffChoicePanel.gameObject.SetActive(false);
        buffChoicePanelButton.onClick.AddListener(() => ToggleBuffChoicePanel());
        button.onClick.AddListener(() => TogglePanel());
        
        //If closed
        if (GameManager.instance.state == GameManager.gameState.beforeDay)
            InitClosed();
        else
            InitOpen();
    }

    void InitClosed()
    {
        button.enabled = false;
    }

    void InitOpen()
    {
        panel.SetActive(false);
    }


    private void ToggleBuffChoicePanel()
    {
        buffChoicePanel.gameObject.SetActive(!buffChoicePanel.gameObject.activeSelf);
    }
    private void TogglePanel()
    {
        panel.SetActive(!panel.activeSelf);
    }
}
