using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsOpenModal;
    [SerializeField] private GameObject blackScreen;
    [SerializeField] private Button toggleButton;

    private GameObject activeSettingsModal;

    void Start()
    {
        if (GameManager.instance.state == GameManager.gameState.startDay)
        {
            activeSettingsModal = settingsOpenModal;
;           toggleButton.onClick.AddListener(() => activeSettingsModal.SetActive(!activeSettingsModal.activeSelf));
        }

    }
}
