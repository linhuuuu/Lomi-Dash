using UnityEngine;
using PCG;
using System;
using UnityEngine.UIElements;

public class PrepTray : MonoBehaviour
{
    public TrayRootNode trayNode { private set; get; }
    public bool isLarge { private set; get; }
    [SerializeField] private int maxDishWeight;
    [SerializeField] private int maxBevWeight;
    [SerializeField] private int maxSeasoningTrayWeight;
    public DishSectionNode[] dishList { private set; get; }
    public BeverageSectionNode[] bevList { private set; get; }
    public SeasoningTraySectionNode seasoningTray { private set; get; } = new SeasoningTraySectionNode();

    //Parts
    [SerializeField] DishSlot[] dishes;
    [SerializeField] BevSlot[] beverages;
    [SerializeField] SeasoningSlot seasoningSlot;

    void Start()
    {
        trayNode = new TrayRootNode();
        dishList = new DishSectionNode[3];
        bevList = new BeverageSectionNode[2];
    }

    public bool AddDish(DishSectionNode dish, int slot)
    {
        dishList[slot] = dish;
        return true;
    }

    public bool RemoveDish(DishSectionNode dish, int slot)
    {
        dishList[slot] = new DishSectionNode();
        return true;
    }

    public bool SwapDish(int first, int second)
    {
        DishSectionNode dishTemp;
        dishTemp = dishList[first];
        dishList[first] = dishList[second];
        dishList[second] = dishTemp;
        return true;
    }

    public bool AddBeverage(BeverageSectionNode bev, int slot)
    {
        bevList[slot] = bev;
        // currentBevWeight += bev.size;
        return true;
    }

    public bool RemoveBev(BeverageSectionNode bev, int slot)
    {
        bevList[slot] = new BeverageSectionNode();
        return true;
    }

    public bool SwapBev(int first, int second)
    {
        BeverageSectionNode bevTemp;
        bevTemp = bevList[first];
        bevList[first] = bevList[second];
        bevList[second] = bevTemp;
        return true;
    }

    public void AddSeasoningTray()
    {
        if (seasoningTray.trayCount < 5)
            seasoningTray.trayCount++;
    }

    public void RemoveSeasoningTray()
    {
        if (seasoningTray.trayCount > 0)
            seasoningTray.trayCount--;
    }

    public OrderNode CompleteTray()
    {
        int i = 0;
        foreach (var dish in dishList)
        {
            if (dish == null) continue;
            dish.id += (i+1).ToString();
            trayNode.children.Add(dish);
            i++;
        }

        i = 0;
        foreach (var bev in bevList)
        {
            if (bev == null) continue;
            bev.id += (i+1).ToString();
            trayNode.children.Add(bev);
            i++;
        }

        trayNode.children.Add(seasoningTray);
        return trayNode;
    }

    public void ClearTray()
    {


        seasoningTray.trayCount = 0;
        trayNode = new();

        foreach (var dish in dishes)
            dish.DestroyDish();

        foreach (var bev in beverages)
            bev.DestroyBev();

        seasoningSlot.RemoveAllStack();

        if (Debug.isDebugBuild) Debug.Log("Cleared Tray");
    }
}