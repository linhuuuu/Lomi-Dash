using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Button[] levelNodes;
    [SerializeField] Button startButton;

    void Start()
    {
        GameManager.instance.state = GameManager.gameState.startDay;
        startButton.onClick.AddListener(() => GameManager.instance.NextScene("Main Screen"));
    }
}
