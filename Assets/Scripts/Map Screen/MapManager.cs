using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject marker;
    [SerializeField] private MapNodes[] mapNodes;

    private MapNodes activeNode;

    public static MapManager map;
    void Awake()
    {
        map = this;
    }

    void Start()
    {
        if (GameManager.instance.state == GameManager.gameState.tutorial)
        {
            backButton.gameObject.SetActive(false);
            GameManager.instance.state = GameManager.gameState.startDay;
            Debug.Log("Saved as startday");
        }

        SetActiveNode(mapNodes[DataManager.data.playerData.highestLevelCleared]);
        
        float totalFame = DataManager.data.playerData.happiness;
        int highestLevel = DataManager.data.playerData.highestLevelCleared;

        foreach (MapNodes nodes in mapNodes)
            nodes.Init(totalFame, highestLevel);

        if (highestLevel > 0)
            CameraDragZoomControl.instance.CenterOnTarget(mapNodes[highestLevel].transform);
        else
            CameraDragZoomControl.instance.CenterOnTarget(mapNodes[0].transform);

        startButton.onClick.AddListener(() => NextScene());
        backButton.onClick.AddListener(() => GameManager.instance.NextScene("Main Screen"));
    }

    public void SetActiveNode(MapNodes node)
    {
        activeNode = node;
        GameManager.instance.roundProfile = node.profile;

        //Move
        // float smoothTime = 0.3f;
        Vector3 velocity = Vector3.zero;

        CameraDragZoomControl.instance.CenterOnTarget(node.transform);
        Vector3 targetPos = node.transform.position + new Vector3(0f, 0.5f, 0f);
        marker.transform.position = targetPos;
    }

    void NextScene()
    {
        GameManager.instance.state = GameManager.gameState.startDay;
        GameManager.instance.NextScene("Main Screen");
        PlayerPrefs.SetInt("lastLevel", activeNode.profile.level);
    }

}
