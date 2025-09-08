using UnityEngine;
using PCG;
using System;
public class PrepTray : DragAndDrop
{
    public TrayRootNode trayNode { private set; get; }
    public bool isLarge { private set; get; }
    [SerializeField] private int maxDishWeight;
    [SerializeField] private int maxBevWeight;
    [SerializeField] private int maxSeasoningTrayWeight;
    private int currentDishWeight, currentBevWeight;
    public DishSectionNode[] dishList { private set; get; }
    public BeverageSectionNode[] bevList { private set; get; }
    public SeasoningTraySection seasoningTray { private set; get; } = new SeasoningTraySection();

    void Start()
    {
        isLarge = DataManager.data.playerData.largeTrayUnlocked;
        trayNode = new TrayRootNode();
        dishList = new DishSectionNode[isLarge ? 3 : 2];
        bevList = new BeverageSectionNode[isLarge ? 3 : 2];

    }

    private int GetDishWeight(DishSectionNode dish)
    {
        return dish.isLarge ? 2 : 1;
    }
    public bool AddDish(DishSectionNode dish, int slot) 
    {
        int dishWeight = GetDishWeight(dish);

        if ((maxDishWeight - currentDishWeight) < dishWeight)
        {
            if (Debug.isDebugBuild) Debug.Log("Dish Max Capacity Reached");
            return false;
        }

        dishList[slot] = dish;
        currentDishWeight += dishWeight;
        return true;
    }

    public bool RemoveDish(DishSectionNode dish, int slot)
    {
        int dishWeight = GetDishWeight(dish);

        dishList[slot] = new DishSectionNode();
        currentDishWeight -= dishWeight;
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
        if ((maxBevWeight - currentBevWeight) < bev.size)
        {
            if (Debug.isDebugBuild) Debug.Log("Bev Max Capacity Reached");
            return false;
        }

        bevList[slot] = bev;
        currentBevWeight += bev.size;
        return true;
    }

    public bool RemoveBev(BeverageSectionNode dish, int slot)
    {
        // int bevWeight = GetBevWeight(bev);

        bevList[slot] = new BeverageSectionNode("Remove this in the future");
        // currentDishWeight -= bevWeight;
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

    public void CompleteTray()
    {
        foreach (var dish in dishList) trayNode.children.Add(dish);
        foreach (var bev in bevList) trayNode.children.Add(bev);
        trayNode.children.Add(seasoningTray);
    }

    public void SubmitTray()
    {
        //Clear 
        Array.Clear(dishList, 0, dishList.Length);
        Array.Clear(bevList, 0, bevList.Length);
        currentDishWeight = 0;
        currentBevWeight = 0;
        seasoningTray.trayCount = 0;

        if (Debug.isDebugBuild) Debug.Log("Cleared Tray");

    }

}