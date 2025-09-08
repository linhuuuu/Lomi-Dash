using PCG;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

// Dish Generation Rules:
// Each Customer must have a bowl;
// Base Tray = 3 slots, Upgraded Tray = 5 slots
// Regular Dish = 1 slot, Large Dish = 2 slots
// LargeDishCount scales with difficulty
// LargeDish can accomodate 2 heads 
// Tray can accomodate max 5 slots, maybe increase to 6?
// 3 Difficulty Level 

public static class OrderGenerator
{
    public static OrderNode GenerateTray(int difficulty, int headCount, bool largeBowlUnlocked, bool largeTrayUnlocked, List<Beverage> bevList, List<Recipe> recipeList)
    {
        //Tray Init
        int trayDishSlots = largeTrayUnlocked ? 5 : 3;
        var trayNode = new TrayRootNode() { weight = 100f };

        //Large Dish Init
        int dishCount = Mathf.Clamp(headCount, 0, 3);
        int largeDishCount = 0;
        if (headCount == 1)
        {
            if (largeBowlUnlocked && ProceduralRNG.Bool(0.1f + difficulty * 0.3f))
                largeDishCount = 1;
            else
                largeDishCount = 0;
        }
        else if (headCount == 2)
        {
            float sharedChance = 0.1f + difficulty * 0.2f; // increases with difficulty
            float twoLargeChance = 0.1f + difficulty * 0.3f;

            if (largeBowlUnlocked && ProceduralRNG.Bool(sharedChance))
                largeDishCount = 1;
            else if (largeBowlUnlocked && ProceduralRNG.Bool(twoLargeChance) && trayDishSlots >= 4)
                largeDishCount = 2;
            else
                largeDishCount = 0;
        }
        else if (headCount == 3)
        {
            float largeChance = 0.1f + difficulty * 0.3f;

            for (int i = 0; i < dishCount; i++)
            {
                if (!largeBowlUnlocked) break;
                if (trayDishSlots > 3 && largeDishCount < 2)
                {
                    if (ProceduralRNG.Bool(largeChance))
                        largeDishCount++;
                }
                else
                {
                    largeDishCount++;
                    break;
                }
            }
        }
        else if (headCount >= 4)
        {
            largeDishCount = 2;
        }

        
        //Dish Generation
        int remainingLarge = largeDishCount;
        int totalSlotsUsed = 0;

        for (int i = 0; i < dishCount; i++)
        {
            bool shouldMakeLarge = remainingLarge > 0;
            var recipe = recipeList[ProceduralRNG.Range(0, recipeList.Count)];

            if (shouldMakeLarge && totalSlotsUsed + 2 <= trayDishSlots)
            {
                trayNode.children.Add(GenerateDish(difficulty, isLarge: true, recipe));
                totalSlotsUsed += 2;
                remainingLarge--;
            }
            else if (totalSlotsUsed + 1 <= trayDishSlots)
            {
                trayNode.children.Add(GenerateDish(difficulty, isLarge: false, recipe));
                totalSlotsUsed += 1;
            }
        }

        // Add Beverage fix
        int beverageCount = ProceduralRNG.Range(1, 2);
        for (int i = 0; i < beverageCount; i++)
        {
            var beverage = bevList[ProceduralRNG.Range(0, bevList.Count)];
            trayNode.children.Add(GenerateBeverage(beverage));
        }

        //Add Seasoning
        trayNode.children.Add(GenerateSeasoning(headCount));

        //Add Weights
        AddWeights(trayNode, 100f);
        
        return trayNode;
    }

    public static OrderNode GenerateDish(int difficulty, bool isLarge, Recipe recipe)
    {
        var dish = new DishSectionNode();
        dish.isLarge = isLarge;

        dish.children.Add(GeneratePotGroup(isLarge));
        dish.children.Add(GenerateWokGroup(isLarge));
        dish.children.Add(GenerateToppingGroup(recipe));

        return dish;
    }

    public static OrderNode GeneratePotGroup(bool isLarge)
    {
        PotGroup pot = new PotGroup();

        int boilTime = 15, waterHeld = 1, bonesCount = 1, saltCount = 1, pepperCount = 1, bawangCount = 1;

        if (isLarge)
        {
            waterHeld = 2;
            saltCount = 2;
            pepperCount = 2;
            bawangCount = 2;
        }

        pot.children.Add(new BoilNode(boilTime, waterHeld));
        pot.children.Add(new BonesNode(bonesCount));
        pot.children.Add(new SeasoningNode(saltCount, pepperCount, bawangCount));

        return pot;
    }

    public static OrderNode GenerateWokGroup(bool isLarge)
    {
        var wok = new WokGroup();
        int oilCount = 1, onionCount = 1, bawangCount = 1, sauteeCount = 1;
        int noodleCount = 1, cookTime = 15, eggCount = 1, thickenerCount = 1;
        bool isMixed = true;

        if (isLarge)
        {
            oilCount = 2; onionCount = 2; bawangCount = 2; sauteeCount = 2;
            noodleCount = 2; eggCount = 2; thickenerCount = 2;
        }

        wok.children.Add(new SauteeNode(oilCount, onionCount, bawangCount, sauteeCount));
        wok.children.Add(new NoodlesNode(noodleCount, cookTime));
        wok.children.Add(new Mix_1_Node(isMixed));
        wok.children.Add(new EggNode(eggCount));
        wok.children.Add(new ThickenerNode(thickenerCount));
        wok.children.Add(new Mix_2_Node(isMixed));

        return wok;
    }

    public static OrderNode GenerateToppingGroup(Recipe recipe)
    {
        var toppingsSection = new ToppingGroup();
        var toppingList = recipe.toppingList;

        foreach(var toppingEntry in toppingList)
            toppingsSection.children.Add(new ToppingNode(toppingEntry.topping, toppingEntry.count));
        return toppingsSection;
    }

    private static OrderNode GenerateBeverage(Beverage bev)
    {
        return new BeverageSectionNode(bev);
    }

    private static OrderNode GenerateSeasoning(int headCount)
    {
        return new SeasoningTraySection { trayCount = headCount };
    }

    //Weight Distribution
    private static void AddWeights(OrderNode rootNode, float totalWeight)
    {
        float dishTotal = totalWeight * 0.7f;
        float beverageTotal = totalWeight * 0.2f;
        float seasoningTrayTotal = totalWeight * 0.1f;
        List<OrderNode> dishes = new List<OrderNode>();
        List<OrderNode> beverages = new List<OrderNode>();

        foreach (var child in rootNode.children)
        {
            switch (child.id)
            {
                case "DISH_SECTION":
                    dishes.Add(child);
                    break;
                case "BEVERAGE_SECTION":
                    beverages.Add(child);
                    break;
                case "SEASONING_TRAY_SECTION":
                    child.weight = seasoningTrayTotal;
                    break;
            }
        }

        DistributeWeightsToDish(dishes, dishTotal);
        DistributeWeightsToBeverages(beverages, beverageTotal);
    }

    private static void DistributeWeightsToBeverages(List<OrderNode> beverages, float beverageTotal)
    {
        float perBev = beverageTotal / beverages.Count;
        foreach (var bev in beverages)
            bev.weight = perBev;
    }

    private static void DistributeWeightsToDish(List<OrderNode> dishes, float dishTotal)
    {
        float perDish = dishTotal / dishes.Count;
        foreach (var dish in dishes)
        {
            dish.weight = perDish;
            DistributeToDishSection(dish);
        }
    }

    private static void DistributeToDishSection(OrderNode dish)
    {
        float[] dishChildrenWeights = new float[3] { 0.3f, 0.4f, 0.3f };    //pot, wok, toppings
        float dishWeight = dish.weight;
        foreach(var section in dish.children)
        {
            switch (section)
            {
                case PotGroup:
                    section.weight = dishWeight * dishChildrenWeights[0];
                    break;
                case WokGroup:
                    section.weight = dishWeight * dishChildrenWeights[1];
                    break;
                case ToppingGroup:
                    section.weight = dishWeight * dishChildrenWeights[2];
                    break;  
            }

            float perChild = section.weight / section.children.Count;
            foreach (var node in section.children)
                node.weight = perChild;
        }
    }
}




