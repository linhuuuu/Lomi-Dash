using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Rendering;

public class ResultScreenManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private Image portrait;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private TextMeshProUGUI totalDishes;
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

    // [SerializeField] private bool isDebug = false;

    private int nextDay;
    private float totalMoney;
    private float totalHappiness;
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

        nextDay = playerData.day + 1;
        totalMoney = playerData.money + results.earnedMoney;
        totalHappiness = playerData.happiness + results.earnedHappiness;
        highestLevelCleared = playerData.highestLevelCleared;
        
        if (results.starCount > 0)
            highestLevelCleared = GameManager.instance.roundProfile.level;

        GameManager.instance.state = GameManager.gameState.beforeDay;

        await InitResultManager();
            GameManager gameManager = GameObject.FindAnyObjectByType<GameManager>();
           next.onClick.AddListener(() => gameManager.NextScene("Main Screen"));
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
                    {"highestLevelCleared", highestLevelCleared}
                });

            await DataManager.data.UploadRoundClearData(GameManager.instance.roundProfile.roundName);
            ReflectChanges();
        }
        else

        if (Debug.isDebugBuild) Debug.Log("No Results Found!");

        await DataManager.data.FetchLeaderBoardData(GameManager.instance.roundProfile.roundName);
        ReflectLeaderBoardChanges();

        // StartCoroutine(anim.StartSequence());
    }

    public void ReflectChanges()
    {
        dayText.text = $"Day {playerData.day} Summary";
        totalDishes.text = "0";
        dishesCleared.text = results.dishesCleared.ToString();
        happyCustomers.text = results.happyCustomers.ToString();
        unhappyCustomers.text = results.unhappyCustomers.ToString();
        earnedMoney.text = results.earnedMoney.ToString();
        earnedHappiness.text = results.earnedHappiness.ToString();
        TotalHappiness.text = totalHappiness.ToString();
        TotalMoney.text = totalMoney.ToString();
        ClearTime.text = results.clearTime.ToString();

        DataManager.data.results = new RoundResults();
    }

    public void ReflectLeaderBoardChanges()
    {
        List<DataManager.MoneyLeaderBoardData> moneyLB = DataManager.data.moneylbData;
        List<DataManager.HappinessLeaderBoardData> happinessLB = DataManager.data.happinesslbData;

        if (Debug.isDebugBuild) Debug.Log(moneyLB.Count);
        if (Debug.isDebugBuild) Debug.Log(happinessLB.Count);

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
