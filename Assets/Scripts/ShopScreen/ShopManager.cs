using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopScreenManager : MonoBehaviour
{
    //Buttons
    [SerializeField] private Button returnButton;
    [SerializeField] private Button purchaseOnce;
    [SerializeField] private Button purchaseAll;
    [SerializeField] private Button purchaseMinus;
    [SerializeField] private Button purchaseAdd;

    //Screen References
    [SerializeField] private GameObject activeScreen;
    [SerializeField] private GameObject inactiveScreen;

    //Header References
    [SerializeField] private TextMeshProUGUI total;
    [SerializeField] private TextMeshProUGUI money;
    [SerializeField] private TextMeshProUGUI voucher;
    [SerializeField] private Button moneyButton;
    [SerializeField] private GameObject moneyCheckMark;
    [SerializeField] private Button voucherButton;
    [SerializeField] private GameObject voucherCheckMark;
    [SerializeField] private Color selectedCurrencyColor;

    //List References
    [SerializeField] private GameObject shopListObjPrefab;
    [SerializeField] private Transform listContainer;

    //Active Screen
    [SerializeField] private TextMeshProUGUI activeName;
    [SerializeField] private TextMeshProUGUI activePrice;
    [SerializeField] private Image activeIcon;
    [SerializeField] private TextMeshProUGUI activeEffects;
    [SerializeField] private TextMeshProUGUI activeDesc;

    [SerializeField] private Dictionary<string, int> buffsAvailable = new();
    List<BuffData> buffDatas = new();
    List<ShopListObj> shopObjs = new();

    private string active;
    private float totalCost;
    private float totalVouchers;
    private bool activeCurrency = false; //0 Money, 1 Voucher

    void Awake()
    {
        buffsAvailable = DataManager.data.playerData.unlockedBuffs;
        returnButton.onClick.AddListener(() => ShopReturnToMainScreen());
    }

    void Start()
    {

        //SetInit

        InitBuffList();
        activeScreen.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(activeScreen.GetComponent<RectTransform>());
    }
    
    void InitBuffList()
    {
        if (buffsAvailable == null || buffsAvailable.Count == 0) return;
        
        int i = 0;
        foreach (string buff in buffsAvailable.Keys)
        {
            BuffData buffData = InventoryManager.inv.gameRepo.BuffsRepo.Find(c => c.id == buff);
            if (buffData == null)
            {
                Debug.Log("Buff not found!");
                continue; 
            }

            ShopListObj shop = Instantiate(shopListObjPrefab, listContainer).GetComponent<ShopListObj>();
            shopObjs.Add(shop);

            shop.InstShopListObj(buffData, i, buffsAvailable[buff]);
            i++;
        }

    }

    // public async void OnPurchaseSingle(float amount)
    // {
    //     // UpdateCurrency(amount, 1);
    //     // await OnPurchase();
    // }

    // async Task OnPurchase()
    // {
    //     //Load waiting Screen
    //     //await Add to Database then playerData

    //     //Update Screens
    //     totalCost = 0;
    //     totalVouchers = 0;
    // }

    void UpdateCurrency(float cost, int amount)
    {
        totalCost += cost;
        totalVouchers += amount;
    }

    void OnChangeCurrency()
    {
        if (activeCurrency == false)
        {
            total.text = $"{totalVouchers} voucher";
            voucherCheckMark.SetActive(true);
            moneyCheckMark.SetActive(false);
        }

        else if (activeCurrency == true)
        {
            total.text = $"PHP {totalCost}";
            voucherCheckMark.SetActive(true);
            moneyCheckMark.SetActive(false);
        }

    }

    void OnSelectCurrency()
    {
        if (activeCurrency != !activeCurrency) return;
        activeCurrency = !activeCurrency;
        OnChangeCurrency();
    }

    void ShopReturnToMainScreen()
    {
        GameManager.instance.NextScene("Main Screen");
    }
}
