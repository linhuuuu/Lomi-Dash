using PCG;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CookWok : DragAndDrop
{
    public bool stove_On = false;
    public SauteeNode sauteeNode { private set; get; }
    public PotGroup potGroup { set; get; }
    public NoodlesNode noodlesNode { private set; get; }
    public SoySauceNode soysauceNode { private set; get; }
    public Mix_1_Node mix_1_Node { private set; get; }
    public ThickenerNode thickenerNode { private set; get; }
    public EggNode eggNode { private set; get; }
    public Mix_2_Node mix_2_Node { private set; get; }
    public WokGroup wokGroup { private set; get; }
    public AnimWok animWok { private set; get; }
    private Coroutine cookingRoutine;

    public int maxCount { set; get; } = 1;

    void Start()
    {
        if (wokGroup == null) wokGroup = new WokGroup();
        animWok = GetComponent<AnimWok>();
        animWok.cookWok = this;
    }

    private void initWok()
    {
        if (wokGroup == null) wokGroup = new WokGroup();
        if (sauteeNode == null) sauteeNode = new SauteeNode();
        if (noodlesNode == null) noodlesNode = new NoodlesNode();
        if (soysauceNode == null) soysauceNode = new SoySauceNode();
        if (mix_1_Node == null) mix_1_Node = new Mix_1_Node();
        if (thickenerNode == null) thickenerNode = new ThickenerNode();
        if (eggNode == null) eggNode = new EggNode();
        if (mix_2_Node == null) mix_2_Node = new Mix_2_Node();
        if (potGroup == null) potGroup = new PotGroup();
    }

    public void ToggleStove()
    {
        stove_On = !stove_On;
        if (Debug.isDebugBuild) Debug.Log("Stove is " + stove_On);
    }

    public void SauteePan(string type)
    {
        if (sauteeNode == null) sauteeNode = new SauteeNode();

        switch (type)
        {
            case "Oil":
                if (sauteeNode.oilCount == maxCount) return;
                sauteeNode.oilCount += maxCount;
                animWok.ToggleOil(true);
                break;
            case "Bawang":
                if (sauteeNode.bawangCount == maxCount) return;
                sauteeNode.bawangCount += maxCount;
                animWok.ToggleBawang(true);
                break;
            case "Onion":
                if (sauteeNode.onionCount == maxCount) return;
                sauteeNode.onionCount += maxCount;
                animWok.ToggleOnion(true);
                break;
            default:
                if (Debug.isDebugBuild) Debug.Log("Unrecognized Type."); break;
        }
    }

    public void AddSoySauce()
    {
        if (soysauceNode == null) soysauceNode = new SoySauceNode();
        
        if (soysauceNode.count == maxCount) return;
        soysauceNode.count += maxCount;
        StartCoroutine(animWok.AddSoySauce());
    }

    public void AddNoodles()
    {
        if (noodlesNode == null) noodlesNode = new NoodlesNode();

        if (noodlesNode.count == maxCount) return;
        noodlesNode.count += maxCount;
        animWok.ToggleNoodles(true);
    }

    public void Mix_1()
    {
        if (mix_1_Node == null) mix_1_Node = new Mix_1_Node();

        if (mix_1_Node.isMixed == true) return;
        mix_1_Node.isMixed = true;
        animWok.MixWok();
    }

    public void AddThickener()
    {
        if (thickenerNode == null) thickenerNode = new ThickenerNode();

        if (thickenerNode.count == maxCount) return;
        thickenerNode.count += maxCount;

        //animRhickener;
    }

    public void AddEgg()
    {
        if (eggNode == null) eggNode = new EggNode();

        if (eggNode.eggCount == maxCount) return;
        eggNode.eggCount += maxCount;

        //animEgg;
    }


    public void Mix_2()
    {
        if (mix_2_Node == null) mix_2_Node = new Mix_2_Node();

        if (mix_2_Node.isMixed == true) return;
        mix_2_Node.isMixed = true;

        //animMix_2;
    }

    public void CreateWokGroup()
    {
        initWok();
        if (wokGroup.children != null) return;
        wokGroup.children = new List<OrderNode>
        {
            sauteeNode,
            noodlesNode,
            soysauceNode,
            mix_1_Node,
            thickenerNode,
            eggNode,
            mix_2_Node
        };
    }

    //Dropping
    public void OnMouseUp()
    {
        initDraggable();

        if (hitCollider == null)
        {
            if (Debug.isDebugBuild) Debug.Log("Got Nothing");
            revertDefaults();
            return;
        }

        if (hitCollider.tag == "Dish")
        {
           
            if (!hitCollider.TryGetComponent(out PrepDish dish)) { revertDefaults();  return;}
            if (dish.wokGroup != null) { revertDefaults(); return; }
            
            wokGroup.children = new List<OrderNode>();

            SauteeNode _sauteeNode = new SauteeNode();
            if (sauteeNode != null)
            {
                if (sauteeNode.oilCount >= 1)
                {
                    _sauteeNode.oilCount++;
                    _sauteeNode.oilTime = sauteeNode.oilTime;
                }

                if (sauteeNode.onionCount >= 1)
                {
                    _sauteeNode.onionCount++;
                    _sauteeNode.onionTime = sauteeNode.onionTime;
                }

                if (sauteeNode.bawangCount >= 1)
                {
                    _sauteeNode.bawangCount++;
                    _sauteeNode.bawangTime = sauteeNode.bawangTime;
                }
            }
            wokGroup.children.Add(_sauteeNode);

            NoodlesNode _noodlesNode = new NoodlesNode();
            if (noodlesNode != null && noodlesNode.count >= 1)
            {
                _noodlesNode.count++;
                _noodlesNode.time = noodlesNode.time;
            }
            wokGroup.children.Add(_noodlesNode);

            SoySauceNode _soySauceNode = new SoySauceNode();
            if (soysauceNode != null && soysauceNode.count >= 1)
            {
                _soySauceNode.count++;
            }
            wokGroup.children.Add(_soySauceNode);

            ThickenerNode _thickenerNode = new ThickenerNode();
            if (thickenerNode != null && thickenerNode.count >= 1)
            {
                _thickenerNode.count++;
            }
            wokGroup.children.Add(_thickenerNode);

            EggNode _eggNode = new EggNode();
            if (eggNode != null && eggNode.eggCount >= 1)
            {
                _eggNode.eggCount++;
            }
            wokGroup.children.Add(_eggNode);

            Mix_1_Node _mix_1_Node = new Mix_1_Node();
            if (mix_1_Node != null)
            {
                _mix_1_Node.isMixed = mix_1_Node.isMixed;
            }
            wokGroup.children.Add(_mix_1_Node);

            Mix_2_Node _mix_2_Node = new Mix_2_Node();
            if (mix_2_Node != null)
            {
                _mix_2_Node.isMixed = mix_2_Node.isMixed;
            }
            wokGroup.children.Add(_mix_2_Node);

            //Transfer Components
            dish.potGroup = potGroup;
            dish.wokGroup = wokGroup;

            //Anim Visuals
            animWok.CreateWok();
            dish.animDish.OnRecieve(animWok.state);

            //Empty List
            wokGroup.children = null;

            // Clear 1 instance — safely decrement and clamp at 0
            if (sauteeNode != null)
            {
                sauteeNode.oilCount = Mathf.Max(0, sauteeNode.oilCount - 1);
                sauteeNode.onionCount = Mathf.Max(0, sauteeNode.onionCount - 1);
                sauteeNode.bawangCount = Mathf.Max(0, sauteeNode.bawangCount - 1);
            }

            if (noodlesNode != null)
                noodlesNode.count = Mathf.Max(0, noodlesNode.count - 1);

            if (soysauceNode != null)
                soysauceNode.count = Mathf.Max(0, soysauceNode.count - 1);

            if (thickenerNode != null)
                thickenerNode.count = Mathf.Max(0, thickenerNode.count - 1);

            if (eggNode != null)
                eggNode.eggCount = Mathf.Max(0, eggNode.eggCount - 1);


            // Clear All instances if everything is zero
            bool allZero =
                (sauteeNode == null || (sauteeNode.oilCount == 0 && sauteeNode.onionCount == 0 && sauteeNode.bawangCount == 0)) &&
                (noodlesNode == null || noodlesNode.count == 0) &&
                (soysauceNode == null || soysauceNode.count == 0) &&
                (thickenerNode == null || thickenerNode.count == 0) &&
                (eggNode == null || eggNode.eggCount == 0);

            if (allZero)
            {
                sauteeNode = null;
                noodlesNode = null;
                soysauceNode = null;
                thickenerNode = null;
                eggNode = null;
                potGroup = null;
                mix_1_Node = null;
                mix_2_Node = null;
                wokGroup = new WokGroup();

                //Clear 
                
                animWok.ToggleActive(false);

                if (Debug.isDebugBuild) Debug.Log("Cleared WokNODE");
            }
            else
            {
                animWok.ReduceWokCount();
                if (Debug.isDebugBuild) Debug.Log("Cleared 1 instance of WokNODE");
            }

            revertDefaults();
            return;
        }
        revertDefaults();
        return;
    }

}