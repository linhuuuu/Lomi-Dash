using System.Runtime.InteropServices.WindowsRuntime;
using PCG;
using UnityEngine;
using UnityEngine.UI;

public class UITray : MonoBehaviour
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

        seasoningTray.SetActive(false);
    }

    public void InitTray(OrderNode order)
    {
        switch (order)
        {
            case DishSectionNode:
                AddDish(order);
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
            seasoningTraySprite.sprite = RoundManager.roundManager.lib.seasoningTrayStates[node.trayCount.ToString()];
    }

    private void AddDish(OrderNode order)
    {
        Recipe _rec = null;
        if (order is not DishSectionNode node) return;

        _rec = InventoryManager.inv.gameRepo.RecipeRepo.Find(c => c.recipeName == node.recipeName);    //!!! CHANGE TO PLAYEREPO ONCE 
        if (_rec == null) return;

        int dishId = order.id[^1] - '0' - 1;

        GameObject currentDish = dish[dishId];
        currentDish.SetActive(true);

        GameObject toppingGroup = Instantiate(_rec.toppingVisual, Vector3.zero, Quaternion.identity, currentDish.transform);
        toppingGroup.transform.SetSiblingIndex(1);
        toppingGroup.transform.localEulerAngles = Vector3.zero;
        toppingGroup.transform.localPosition = Vector3.zero;
    }

    private void AddBev(OrderNode order)
    {
        Beverage _bev = null;
        if (order is not BeverageSectionNode node) return;

        _bev = InventoryManager.inv.gameRepo.BeverageRepo.Find(c => c.bevName == node.name);    //!!! CHANGE TO PLAYEREPO ONCE 
        if (_bev == null) return;

        int bevId = order.id[^1] - '0' - 1;
        bev[bevId].SetActive(true);
        bevSprites[bevId].sprite = _bev.sprite;
    }

        public GameObject[] GetDishUI()
    {
        return this.dish;
    }

    public GameObject[] GetSeasoningUI()
    {
        return this.bev;
    }

    public GameObject GetSeasoningTrayUI()
    {
        return this.seasoningTray;
    }

}