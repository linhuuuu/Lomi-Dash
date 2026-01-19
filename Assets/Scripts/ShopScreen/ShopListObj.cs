using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopListObj : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI listObjName; 
    [SerializeField] private TextMeshProUGUI listObjPrice; 
    [SerializeField] private TextMeshProUGUI listObjOwned;
    [SerializeField] private Image listObjIcon;
    private ShopScreenManager shop;
    private Button btn;
    public int buffDataID { set; get; }
    
    void Awake()
    {
        btn = GetComponent<Button>();

    }

    public void InstShopListObj(ShopScreenManager shop, BuffData data, int buffDataID, int owned)
    {
        this.buffDataID = buffDataID;
        listObjName.text = data.buffName;
        listObjPrice.text = data.price.ToString();
        listObjIcon.sprite = data.sprite;
        this.shop = shop;

        btn.onClick.AddListener(() => shop.SelectActiveObject(buffDataID));
        btn.onClick.AddListener(() => AudioManager.instance.PlayUI(UI.CLICK));

        UpdateOwned(owned);
    }

    public void UpdateOwned(int num) => listObjOwned.text = $"Owned: {num}";
};