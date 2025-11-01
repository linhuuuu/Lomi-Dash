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

    public int buffDataID { set; get; }
    
    void Awake()
    {

    }

    public void InstShopListObj(BuffData data, int buffDataID, int owned)
    {
        this.buffDataID = buffDataID;
        listObjName.text = data.buffName;
        listObjIcon.sprite = data.sprite;

        UpdateOwned(owned);


    }

    public void UpdateOwned(int num) => listObjOwned.text = $"Owned: {num}";
};