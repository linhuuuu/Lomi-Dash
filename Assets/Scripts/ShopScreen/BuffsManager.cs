using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffsManager : MonoBehaviour
{
    [SerializeField] private Button buffChoicePanelButton;
    [SerializeField] private GameObject buffChoicePanel;
    [SerializeField] private List<BuffDrag> buffDrags;

    void Start()
    {
        buffChoicePanel.gameObject.SetActive(false);
        buffChoicePanelButton.onClick.AddListener(() => ToggleBuffChoicePanel());
        
        //If closed
        if (GameManager.instance.state == GameManager.gameState.beforeDay)
            InitClosed();
        else
            InitOpen();
    }

    void InitClosed()
    {
        buffChoicePanelButton.enabled = false;
    }

    void InitOpen()
    {
        buffChoicePanel.SetActive(false);
    }

    private void ToggleBuffChoicePanel()
    {
        buffChoicePanel.gameObject.SetActive(!buffChoicePanel.gameObject.activeSelf);
    }
}
