using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using PCG;
using UnityEngine;
using UnityEngine.UI;

public class UITray : MonoBehaviour
{
    [SerializeField] private GameObject[] dish;
    public List<Recipe> recipes { set; get; } = new();

    [SerializeField] public Image[] bevSprites;
    public List<Beverage> bev { set; get; } = new();

    [SerializeField] private Image seasoningTraySprite;

    private List<bool> sizes = new();

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
        if (seasoningTraySprite.gameObject.activeSelf == false)
            seasoningTraySprite.gameObject.SetActive(true);

        if (order is SeasoningTraySectionNode node && node.trayCount > 0)
            seasoningTraySprite.sprite = RoundManager.roundManager.lib.seasoningTrayStates[node.trayCount.ToString()];
    }

    private void AddDish(OrderNode order)
    {
        Recipe _rec = null;
        if (order is not DishSectionNode node) return;

        _rec = InventoryManager.inv.gameRepo.RecipeRepo.Find(c => c.recipeName == node.recipeName);
        if (_rec == null) return;

        recipes.Add(_rec);
        sizes.Add(node.isLarge);

        int dishId = order.id[^1] - '0' - 1;

        GameObject currentDish = dish[dishId];
        currentDish.SetActive(true);

        //Visual
        GameObject toppingGroup = Instantiate(_rec.toppingVisual, Vector3.zero, Quaternion.identity, currentDish.transform);
        toppingGroup.transform.SetSiblingIndex(1);
        toppingGroup.transform.localEulerAngles = Vector3.zero;
        toppingGroup.transform.localPosition = Vector3.zero;
    }

    private void AddBev(OrderNode order)
    {
        Beverage _bev = null;
        if (order is not BeverageSectionNode node) return;

        _bev = InventoryManager.inv.gameRepo.BeverageRepo.Find(c => c.bevName == node.name);
        if (_bev == null) return;

        bev.Add(_bev);

        int bevId = order.id[^1] - '0' - 1;
        bevSprites[bevId].gameObject.SetActive(true);
        bevSprites[bevId].sprite = _bev.sprite;
    }

    public bool GetSize(int i)
    {
        return this.sizes[i];
    }

}