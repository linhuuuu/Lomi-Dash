using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolManager : MonoBehaviour
{
    [Header("Tool References")]
    [SerializeField] private CookPot pot_1;
    [SerializeField] private CookPot pot_2;
    [SerializeField] private CookWok wok_1;
    [SerializeField] private CookWok wok_2;
    [SerializeField] private GameObject dishRack_1;
    [SerializeField] private GameObject dishRack_2;

    [Header("Size References")]
    [SerializeField] private Vector3 potLargeSize;
    [SerializeField] private Vector3 wokLargeSize;

    [Header("Interface References")]
    [SerializeField] private GameObject upgradeCanvas;
    [SerializeField] private Button pot_1_Upgrade;
    [SerializeField] private Button pot_2_Upgrade;
    [SerializeField] private Button wok_1_Upgrade;
    [SerializeField] private Button wok_2_Upgrade;
    [SerializeField] private Button dishRack_Upgrade;
    [SerializeField] private GameObject moneyPanel;
    [SerializeField] private TextMeshProUGUI moneyText;

    [Header("Upgrade Prices")]
    [SerializeField] private List<float> potUpgradePrices;
    [SerializeField] private List<float> wokUpgradePrices;
    [SerializeField] private float dishRackUpgradePrices;

    private float currMoney;

    private int pot_1_Tier;
    private int pot_2_Tier;
    private int wok_1_Tier;
    private int wok_2_Tier;
    private int dishRack_Tier;

    public void Start()
    {
        pot_1_Tier = 1;
        pot_2_Tier = 0;
        wok_1_Tier = 1;
        wok_2_Tier = 0;
        dishRack_Tier = 1;

        if (DataManager.data.loaded)
        {
            var toolUpgrades = DataManager.data.playerData.unlockedKitchenTools;
            foreach (var tool in toolUpgrades)
            {
                switch (tool.Key)
                {
                    case "Pot_1": pot_1_Tier = tool.Value; break;
                    case "Pot_2": pot_2_Tier = tool.Value; break;
                    case "Wok_1": wok_1_Tier = tool.Value; break;
                    case "Wok_2": wok_2_Tier = tool.Value; break;
                    case "DishRack": dishRack_Tier = tool.Value; break;
                }
            }
        }
        else
            if (Debug.isDebugBuild) Debug.Log("No Data Manager Found!");

        InitPot(pot_1, pot_1_Tier);
        InitPot(pot_2, pot_2_Tier);
        InitWok(wok_1, wok_1_Tier);
        InitWok(wok_2, wok_2_Tier);
        InitDishRack(dishRack_1, dishRack_2, dishRack_Tier);

        if (GameManager.instance.state == GameManager.gameState.beforeDay)
        {
            currMoney = DataManager.data.playerData.money;
            moneyText.text = currMoney.ToString();
            SetupButtonListeners();
            ReloadButtons();
        }
        else
        {
            upgradeCanvas.SetActive(false);
            moneyPanel.SetActive(false);
        }
    }

    void InitPot(CookPot pot, int tier)
    {
        if (tier == 0)
        {
            pot.gameObject.SetActive(false);
        }
        else if (tier == 1)
        {
            pot.gameObject.SetActive(true);
            pot.maxCount = 2;
        }
        else
        {
            pot.gameObject.SetActive(true);
            pot.maxCount = 4;
            pot.transform.localScale = potLargeSize;
        }
    }

    void InitWok(CookWok wok, int tier)
    {
        if (tier == 0)
        {
            wok.gameObject.SetActive(false);
        }
        else if (tier == 1)
        {
            wok.gameObject.SetActive(true);
            wok.maxCount = 1;
        }
        else
        {
            wok.gameObject.SetActive(true);
            wok.maxCount = 2;
            wok.transform.localScale = wokLargeSize;
        }
    }

    void InitDishRack(GameObject rack_1, GameObject rack_2, int tier)
    {
        rack_1.SetActive(tier == 1);
        rack_2.SetActive(tier >= 2);
    }

    void SetupButtonListeners()
    {
        pot_1_Upgrade.onClick.AddListener(() => UpgradeTool("Pot_1", pot_1_Tier, 2, () => InitPot(pot_1, ++pot_1_Tier)));
        pot_2_Upgrade.onClick.AddListener(() => UpgradeTool("Pot_2", pot_2_Tier, 2, () => InitPot(pot_2, ++pot_2_Tier)));
        wok_1_Upgrade.onClick.AddListener(() => UpgradeTool("Wok_1", wok_1_Tier, 2, () => InitWok(wok_1, ++wok_1_Tier)));
        wok_2_Upgrade.onClick.AddListener(() => UpgradeTool("Wok_2", wok_2_Tier, 2, () => InitWok(wok_2, ++wok_2_Tier)));
        dishRack_Upgrade.onClick.AddListener(() => UpgradeTool("DishRack", dishRack_Tier, 2, () => InitDishRack(dishRack_1, dishRack_2, ++dishRack_Tier)));
    }

    async void UpgradeTool(string tool, int currentTier, int maxTier, Action onUpdated)
    {
        if (currentTier >= maxTier) return;

        float price = 0f;

        if (tool.StartsWith("Pot")) price = potUpgradePrices[currentTier];
        else if (tool.StartsWith("Wok")) price = wokUpgradePrices[currentTier];
        else if (tool == "DishRack") price = dishRackUpgradePrices;

        if (currMoney < price) return;

        currMoney -= price;

        var updates = new Dictionary<string, object>
        {
            { "unlockedKitchenTools", new Dictionary<string, int>{ { tool ,currentTier + 1 } } },
            { "money", currMoney }
        };

        await DataManager.data.UpdatePlayerDataAsync(updates);

        onUpdated?.Invoke();
        moneyText.text = currMoney.ToString();
        ReloadButtons();
        AudioManager.instance.PlayUI(UI.PURCHASE);
    }

    void ReloadButtons()
    {
        UpdateButton(pot_1_Upgrade, pot_1_Tier, potUpgradePrices);
        UpdateButton(pot_2_Upgrade, pot_2_Tier, potUpgradePrices);
        UpdateButton(wok_1_Upgrade, wok_1_Tier, wokUpgradePrices);
        UpdateButton(wok_2_Upgrade, wok_2_Tier, wokUpgradePrices);

        dishRack_Upgrade.gameObject.SetActive(dishRack_Tier < 2);
        if (dishRack_Tier < 2)
            dishRack_Upgrade.interactable = currMoney >= dishRackUpgradePrices;
    }

    void UpdateButton(Button button, int tier, List<float> prices)
    {
        button.gameObject.SetActive(tier < 2);
        if (tier < 2)
            button.interactable = currMoney >= prices[tier];

        Debug.Log($"Current Money {currMoney} Price: {prices[tier]}:");
    }
}