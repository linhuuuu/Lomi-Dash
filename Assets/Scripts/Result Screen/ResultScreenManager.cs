using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;
using System;

public class ResultScreenManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private Image portrait;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private TextMeshProUGUI totalDishes;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI dishesCleared;
    [SerializeField] private TextMeshProUGUI happyCustomers;
    [SerializeField] private TextMeshProUGUI unhappyCustomers;
    [SerializeField] private TextMeshProUGUI earnedMoney;
    [SerializeField] private TextMeshProUGUI earnedHappiness;
    [SerializeField] private TextMeshProUGUI TotalMoney;
    [SerializeField] private TextMeshProUGUI TotalHappiness;
    [SerializeField] private TextMeshProUGUI ClearTime;
    [SerializeField] private TextMeshProUGUI errorMessage;

    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform moneyLeaderboardContent;
    [SerializeField] private Transform happinessLeaderboardContent;
    [SerializeField] private AnimResults anim;

    [SerializeField] private Button next;

    [SerializeField] private List<string> unlocks;
    [SerializeField] private GameObject notificationObj;

    private int nextDay;
    private float totalMoney;
    private float totalHappiness;
    private int finalStarCount;
    private int highestLevelCleared;
    private RoundResults results;
    private PlayerSaveData playerData;

    public static ResultScreenManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
    }

    async void Start()
    {
        results = DataManager.data.results;
        if (results == null)
        {
            if (Debug.isDebugBuild) Debug.Log("No Results Found!");
            return;
        }

        playerData = DataManager.data.playerData;

        portrait.sprite = GameManager.instance.roundProfile.specialCustomerUnlock.portrait;

        for (int i = 0; i < results.starCount; i++)
        {
            stars[i].SetActive(true);
        }

        nextDay = playerData.day + 1;
        totalMoney = playerData.money + results.earnedMoney;
        totalHappiness = playerData.happiness + results.earnedHappiness;
        highestLevelCleared = playerData.highestLevelCleared;

        if (results.starCount > 0)
            highestLevelCleared = GameManager.instance.roundProfile.level;

        if (highestLevelCleared < DataManager.data.playerData.highestLevelCleared)
        {
            highestLevelCleared = DataManager.data.playerData.highestLevelCleared;
        }

        finalStarCount = Mathf.Max(results.starCount, playerData.clearStars[GameManager.instance.roundProfile.roundName]);

        GameManager.instance.state = GameManager.gameState.beforeDay;
        next.onClick.AddListener(() => GameManager.instance.NextScene("Main Screen"));

        await InitResultManager();
    }

    public async Task InitResultManager()
    {
        if (results != null)
        {
            await DataManager.data.UpdatePlayerDataAsync(new Dictionary<string, object>
                {
                    {"day", nextDay},
                    {"money", totalMoney},
                    {"happiness", totalHappiness},
                    {"totalMoney", playerData.totalMoney + results.earnedMoney},
                    {"totalHappiness", playerData.totalHappiness + results.earnedHappiness},
                    {"totalStagesCleared",  playerData.totalStagesCleared + 1},
                    {"latestStageCleared", GameManager.instance.roundProfile.roundName},
                    {"highestLevelCleared", highestLevelCleared},
                    {"clearStars", new Dictionary<string, int>{ { GameManager.instance.roundProfile.roundName, finalStarCount } } },
                });

            await DataManager.data.UploadRoundClearData(GameManager.instance.roundProfile.roundName);
            ReflectChanges();
        }
        else
            if (Debug.isDebugBuild) Debug.Log("No Results Found!");

        await DataManager.data.FetchLeaderBoardData(GameManager.instance.roundProfile.roundName);
        ReflectLeaderBoardChanges();
        StartCoroutine(anim.StartSequence());
    }

    public void ReflectChanges()
    {
        dayText.text = $"Day {playerData.day} Summary";
        scoreText.text = results.score.ToString();
        totalDishes.text = results.totalDishes.ToString();
        dishesCleared.text = results.dishesCleared.ToString();
        happyCustomers.text = results.happyCustomers.ToString();
        unhappyCustomers.text = results.unhappyCustomers.ToString();
        earnedMoney.text = results.earnedMoney.ToString();
        earnedHappiness.text = results.earnedHappiness.ToString();
        TotalHappiness.text = totalHappiness.ToString();
        TotalMoney.text = totalMoney.ToString();
        ClearTime.text = (Mathf.Round(results.clearTime * 100) / 100).ToString() + "s";

        DataManager.data.results = new RoundResults();
    }

    public void ReflectLeaderBoardChanges()
    {
        List<DataManager.MoneyLeaderBoardData> moneyLB = DataManager.data.moneylbData;
        List<DataManager.HappinessLeaderBoardData> happinessLB = DataManager.data.happinesslbData;
 
        for (int i = 0; i < happinessLB.Count; i++)
        {
            var happinessItem = happinessLB[i];
            GameObject item = Instantiate(prefab, Vector3.zero, Quaternion.identity, happinessLeaderboardContent);
            item.GetComponent<LeaderBoardItem>().InitLeaderboardItem(i + 1, happinessItem.ply.name, happinessItem.happiness, happinessItem.ply.icon);
            item.transform.localScale = Vector3.one;
        }

        for (int i = 0; i < moneyLB.Count; i++)
        {
            var moneyItem = moneyLB[i];
            GameObject item = Instantiate(prefab, Vector3.zero, Quaternion.identity, moneyLeaderboardContent);
            item.GetComponent<LeaderBoardItem>().InitLeaderboardItem(i + 1, moneyItem.ply.name, moneyItem.money, moneyItem.ply.icon);
            item.transform.localScale = Vector3.one;
        }

        //Reset Data
        DataManager.data.moneylbData = new();
        DataManager.data.happinesslbData = new();
    }
}
