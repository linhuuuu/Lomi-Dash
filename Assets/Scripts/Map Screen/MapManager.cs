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

    [SerializeField] private TextMeshProUGUI levelName;
    [SerializeField] private TextMeshProUGUI difficulty;
    [SerializeField] private Image featuredCustomer;
    [SerializeField] private TextMeshProUGUI fameToUnlock;

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
        }

        float totalFame = DataManager.data.playerData.happiness;
        int highestLevel = Mathf.Clamp(DataManager.data.playerData.highestLevelCleared, 1, 3);

        foreach (MapNodes nodes in mapNodes)
            nodes.Init(totalFame, highestLevel);

        if (highestLevel > 0)
            CameraDragZoomControl.instance.CenterOnTarget(mapNodes[highestLevel-1].transform);
        else
            CameraDragZoomControl.instance.CenterOnTarget(mapNodes[0].transform);

            SetActiveNode(mapNodes[highestLevel-1]);


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

        levelName.text = GameManager.instance.roundProfile.roundName;
        difficulty.text = $"Difficulty: {GameManager.instance.roundProfile.difficulty}";
        featuredCustomer.sprite = GameManager.instance.roundProfile.specialCustomerUnlock.portrait;

        float currHappiness = DataManager.data.playerData.happiness;
        float requiredFame = InventoryManager.inv.gameRepo.roundProfiles[GameManager.instance.roundProfile.level].requiredFame;
        if (currHappiness >= requiredFame)
            fameToUnlock.text = "";
        else
            fameToUnlock.text = $"Gain +{requiredFame - currHappiness} Fame to unlock next stage!";
    }

    void NextScene()
    {
        GameManager.instance.state = GameManager.gameState.startDay;
        GameManager.instance.NextScene("Main Screen");
        PlayerPrefs.SetInt("lastLevel", activeNode.profile.level);
    }

}
