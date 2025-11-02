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
    [SerializeField] private Button purchaseAll;
    [SerializeField] private Button purchaseMinus;
    [SerializeField] private Button purchaseAdd;

    //Screen References
    [SerializeField] private GameObject activeScreen;
    [SerializeField] private GameObject inactiveScreen;
    [SerializeField] private TextMeshProUGUI inactiveScreenText;

    //Header References
    [SerializeField] private TextMeshProUGUI total;
    [SerializeField] private TextMeshProUGUI money;
    [SerializeField] private TextMeshProUGUI voucher;
    [SerializeField] private Button moneyButton;
    [SerializeField] private GameObject moneyCheckMark;
    [SerializeField] private Button voucherButton;
    [SerializeField] private GameObject voucherCheckMark;

    //List References
    [SerializeField] private GameObject shopListObjPrefab;
    [SerializeField] private Transform listContainer;

    //Active Screen
    [SerializeField] private TextMeshProUGUI activeName;
    [SerializeField] private TextMeshProUGUI activePrice;
    [SerializeField] private Image activeIcon;
    [SerializeField] private TextMeshProUGUI activeEffects;
    [SerializeField] private TextMeshProUGUI activeDesc;
    [SerializeField] private TextMeshProUGUI activeOwned;
    [SerializeField] private TextMeshProUGUI purchaseAmount;

    [SerializeField] private Dictionary<string, int> buffsAvailable = new();
    List<BuffData> buffDatas = new();
    List<ShopListObj> shopListObjs = new();

    private BuffData active;
    private int activeIndex;
    private float totalCost;
    private float totalVouchers;
    private int inCartAmount;
    private bool activeCurrency = false; //0 Money, 1 Voucher

    void Awake()
    {
        buffsAvailable = DataManager.data.playerData.unlockedBuffs;
    }

    void Start()
    {
        //SetInit
        InitBuffList();
        activeScreen.SetActive(false);
        inactiveScreen.SetActive(true);

        InitCurrencies();

        //Select
        activeCurrency = false;
        voucherCheckMark.SetActive(false);
        moneyCheckMark.SetActive(true);

        returnButton.onClick.AddListener(() => ShopReturnToMainScreen());
        moneyButton.onClick.AddListener(() => OnChangeCurrency(false));
        voucherButton.onClick.AddListener(() => OnChangeCurrency(true));
        purchaseAll.onClick.AddListener(() => OnPurchaseAll());
        purchaseAdd.onClick.AddListener(() => OnAddCart(active.price));
        purchaseMinus.onClick.AddListener(() => OnMinusCart(active.price));

        moneyButton.onClick.AddListener(() => AudioManager.instance.PlayUI(UI.CLICK));
        voucherButton.onClick.AddListener(() => AudioManager.instance.PlayUI(UI.CLICK));
        purchaseAdd.onClick.AddListener(() => AudioManager.instance.PlayUI(UI.CLICK));
        purchaseMinus.onClick.AddListener(() => AudioManager.instance.PlayUI(UI.CLICK));
        purchaseAll.onClick.AddListener(() => AudioManager.instance.PlayUI(UI.CLICK));

        IsCurrencyEnough();

        LayoutRebuilder.ForceRebuildLayoutImmediate(activeScreen.GetComponent<RectTransform>());
    }

    void InitCurrencies()
    {
        money.text = DataManager.data.playerData.money.ToString();
        voucher.text = DataManager.data.playerData.voucher.ToString();
        total.text = $" PHP 0";
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
            buffDatas.Add(buffData);

            ShopListObj shop = Instantiate(shopListObjPrefab, listContainer).GetComponent<ShopListObj>();
            shop.InstShopListObj(this, buffData, i, buffsAvailable[buff]);
            shopListObjs.Add(shop);
            i++;
        }
    }

    public void SelectActiveObject(int i)
    {
        activeScreen.SetActive(true);
        inactiveScreen.SetActive(false);

        BuffData data = buffDatas[i];
        int owned = buffsAvailable[data.id];
        activeIndex = i;
        active = data;

        totalCost = 0;
        totalVouchers = 0;

        inCartAmount = 0;

        OnUpdateAmount();

        activeName.text = data.buffName;
        activePrice.text = data.price.ToString();
        activeIcon.sprite = data.sprite;
        activeEffects.text = data.effect;
        activeDesc.text = "";
        activeOwned.text = $"Owned: {owned}";
    }

    async Task OnPurchase()
    {
        //Load waiting Screen
        Dictionary<string, object> update = new Dictionary<string, object> { { "unlockedBuffs", new Dictionary<string, int> { { active.id, buffsAvailable[active.id] + inCartAmount } } } };

        if (activeCurrency == false)
            update.Add("money", DataManager.data.playerData.money - totalCost);
        else
            update.Add("voucher", DataManager.data.playerData.voucher - totalVouchers);

        await DataManager.data.UpdatePlayerDataAsync(update);

        StartCoroutine(PurchaseNotif());

        //Reload Data
        InitCurrencies();
        buffsAvailable = DataManager.data.playerData.unlockedBuffs;
        activeOwned.text = $"Owned: {buffsAvailable[active.id]}";
        shopListObjs[activeIndex].UpdateOwned(buffsAvailable[active.id]);

        //Reset Data
        totalCost = 0;
        totalVouchers = 0;
        inCartAmount = 0;
        IsCurrencyEnough();
        OnUpdateAmount();
    }

    IEnumerator PurchaseNotif()
    {
        AudioManager.instance.PlayUI(UI.PURCHASE);

        activeScreen.gameObject.SetActive(false);
        inactiveScreenText.text = $"Purchased! {active.buffName} x {inCartAmount}";
        inactiveScreen.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        inactiveScreen.gameObject.SetActive(false);
        activeScreen.gameObject.SetActive(true);
    }

    void IsCurrencyEnough()
    {
        purchaseAdd.enabled = true;
        purchaseAll.enabled = false;
        purchaseMinus.enabled = false;

        if (inCartAmount > 0)
        {
            purchaseMinus.enabled = true;
            purchaseAll.enabled = true;
        }

        if (!activeCurrency)
        { if (totalCost + active.price <= DataManager.data.playerData.money) return; }
        else
        { if (totalVouchers + 1 <= DataManager.data.playerData.voucher) return; }

        purchaseAdd.enabled = false;

        //Check if overflow and reset to closest
        if (totalCost > DataManager.data.playerData.money || totalVouchers > DataManager.data.playerData.voucher)
        {
            if (!activeCurrency)
                while (totalCost > DataManager.data.playerData.money && inCartAmount > 0)
                {
                    totalCost -= active.price;
                    totalVouchers--;
                    inCartAmount--;
                }

            else
                while (totalVouchers > DataManager.data.playerData.voucher && inCartAmount > 0)
                {
                    totalCost -= active.price;
                    totalVouchers--;
                    inCartAmount--;
                }
        }

        //Recheck
        if (inCartAmount < 1)
        {
            purchaseMinus.enabled = false;
            purchaseAll.enabled = false;
            purchaseAdd.enabled = true;
        }
    }

    public void OnAddCart(float amount)
    {
        inCartAmount++;
        UpdateCurrency(amount, 1);

        AudioManager.instance.PlayUI(UI.CLICK);
    }

    public void OnMinusCart(float amount)
    {
        inCartAmount = Mathf.Max(inCartAmount -1, 0);
        UpdateCurrency(-amount, -1);

        AudioManager.instance.PlayUI(UI.CLICK);
    }

    public async void OnPurchaseAll() => await OnPurchase();

    void OnChangeCurrency(bool type)
    {
        if (activeCurrency == type) return;
        activeCurrency = !activeCurrency;

        if (activeCurrency == true)
        {
            voucherCheckMark.SetActive(true);
            moneyCheckMark.SetActive(false);
        }

        else if (activeCurrency == false)
        {
            voucherCheckMark.SetActive(false);
            moneyCheckMark.SetActive(true);
        }

        IsCurrencyEnough();
        OnUpdateAmount();
    }

    void UpdateCurrency(float cost, int amount)
    {
        totalCost = Mathf.Clamp(totalCost+cost, 0, DataManager.data.playerData.money);
        totalVouchers = Mathf.Clamp(totalVouchers+amount, 0, DataManager.data.playerData.voucher);

        IsCurrencyEnough();
        OnUpdateAmount();
    }

    void OnUpdateAmount()
    {
        if (activeCurrency == false)
            total.text = $"PHP {totalCost}";
        else
            total.text = $"{totalVouchers} vouchers";

        purchaseAmount.text = $"Purchase x{inCartAmount}";
    }

    void ShopReturnToMainScreen()
    {
        GameManager.instance.NextScene("Main Screen");
    }
}
