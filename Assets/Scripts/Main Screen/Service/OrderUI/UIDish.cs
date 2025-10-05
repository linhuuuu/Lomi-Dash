using PCG;
using UnityEngine;
using UnityEngine.UI;

public class UIDish : MonoBehaviour
{
    [SerializeField] private GameObject[] dish;
    [SerializeField] private GameObject[] bev;
    [SerializeField] private GameObject seasoningTray;
    private Image[] dishSprites;
    private Image[] bevSprites;
    private Image seasoningTraySprite;

    void Awake()
    {
        dishSprites = new Image[dish.Length];
        bevSprites = new Image[bev.Length];
        seasoningTraySprite = seasoningTray.GetComponent<Image>();

        for (int i = 0; i < dish.Length; i++)
            dishSprites[i] = dish[i].GetComponent<Image>();

        for (int i = 0; i < bev.Length; i++)
            bevSprites[i] = bev[i].GetComponent<Image>();

        foreach (var d in dish)
            d.SetActive(false);

        foreach (var b in bev)
            b.SetActive(false);

        seasoningTray.SetActive(false);
    }

    public void InitTray(OrderNode order)
    {
        switch (order)
        {
            case DishSectionNode:
                //AddDish(order);
                break;
            case BeverageSectionNode:
                AddBev(order);
                break;
            case SeasoningTraySectionNode:
                InitSeasoningTray(order);
                break;
        }

        if (order.children == null) return;

        foreach (OrderNode child in order.children)
        {
            InitTray(child);
        }
    }

    private void InitSeasoningTray(OrderNode order)
    {
        if (seasoningTray.activeSelf == false)
            seasoningTray.SetActive(true);

        if (order is SeasoningTraySectionNode node && node.trayCount > 0)
            seasoningTraySprite.sprite = VisualStateLib.lib.seasoningTrayStates[node.trayCount.ToString()];
    }

    private void AddDish(OrderNode order)
    {
        int dishId = order.id[order.id.Length - 1];

    }

    private void AddBev(OrderNode order)
    {
        Beverage _bev = null;
        if (order is not BeverageSectionNode node) return;

        _bev = InventoryManager.inv.gameRepo.BeverageRepo.Find(c => c.bevName == node.name);    //!!! CHANGE TO PLAYEREPO ONCE 
        if (_bev == null) return;

        int bevId = order.id[^1] - '0' - 1;
        Debug.Log(_bev);
        Debug.Log(bevId);


        bev[bevId].SetActive(true);
        bevSprites[bevId].sprite = _bev.sprite;

        Debug.Log(bevSprites);
            Debug.Log(_bev.sprite);
    }
}