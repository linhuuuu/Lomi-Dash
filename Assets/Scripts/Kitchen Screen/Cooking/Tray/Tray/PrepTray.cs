using UnityEngine;
using PCG;
using System;
using System.Linq;
using System.Data.Common;
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
    public SeasoningTraySectionNode seasoningTray { private set; get; } = new SeasoningTraySectionNode();

    //Parts
    [SerializeField] Transform[] dishes;
    [SerializeField] Transform[] beverages;
    [SerializeField] Transform[] seasoningTrays;

    void Start()
    {
        isLarge = true;
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

        bevList[slot] = new BeverageSectionNode();
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
        //Clear 
        Array.Clear(dishList, 0, dishList.Length);
        Array.Clear(bevList, 0, bevList.Length);
        currentDishWeight = 0;
        currentBevWeight = 0;
        seasoningTray.trayCount = 0;

        //Imporve the following because they should do pooling and not instantiating
        foreach (var dish in dishes)
            foreach(var d in dish.gameObject.GetComponentsInChildren<PrepDish>())
                Destroy(d.gameObject);

        foreach (var bev in beverages)
            foreach(var d in bev.gameObject.GetComponentsInChildren<PrepBev>())
                Destroy(d.gameObject);

        foreach (var tray in seasoningTrays)
            foreach(var d in tray.gameObject.GetComponentsInChildren<SeasoningTray>())
                Destroy(d.gameObject);

        if (Debug.isDebugBuild) Debug.Log("Cleared Tray");

    }

}