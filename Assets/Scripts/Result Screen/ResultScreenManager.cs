using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    private int nextDay;
    private float totalMoney;
    private float totalHappiness;
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

    void Start()
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

        GameManager.instance.state = GameManager.gameState.beforeDay;
    }

    public async Task InitResultManager()
    {
        if (results == null)
        {
            if (Debug.isDebugBuild) Debug.Log("No Results Found!");
            return;
        }

        await DataManager.data.UpdatePlayerDataAsync(new Dictionary<string, object>
        {
            {"day", nextDay},
            {"money", totalMoney},
            {"happiness", totalHappiness},
        //await ingredients
        });

        await DataManager.data.UploadRoundClearData(GameManager.instance.roundProfile.roundName);

        ReflectChanges();
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
}
