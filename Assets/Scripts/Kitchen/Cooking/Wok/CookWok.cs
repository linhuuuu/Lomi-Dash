using PCG;
using System.Collections.Generic;
using UnityEngine;

public class CookWok : DragAndDrop
{
    public bool stove_On = false;
    public SauteeNode sauteeNode { private set; get; }
    public PotGroup potGroup { set; get; }
    public NoodlesNode noodlesNode { private set; get; }
    public Mix_1_Node mix_1_Node { private set; get; }
    public ThickenerNode thickenerNode { private set; get; }
    public EggNode eggNode { private set; get; }
    public Mix_2_Node mix_2_Node { private set; get; }
    public WokGroup wokGroup { private set; get; }
    private Coroutine cookingRoutine;

    void Start()
    {
         if (wokGroup == null) wokGroup = new WokGroup();
    }
    private void initWok()
    {
        if (wokGroup == null) wokGroup = new WokGroup();
        if (sauteeNode == null) sauteeNode = new SauteeNode();
        if (noodlesNode == null) noodlesNode = new NoodlesNode();
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
                sauteeNode.oilCount++; break;
            case "Bawang":
                sauteeNode.bawangCount++; break;
            case "Onion":
                sauteeNode.onionCount++; break;
            default:
                if (Debug.isDebugBuild) Debug.Log("Unrecognized Type."); break;
        }
        if (Debug.isDebugBuild) Debug.Log("Added Sautee Component of Type " + type);

        sauteeNode.satueeCount++; //improve
    }

    public void AddNoodles()
    {
        if (noodlesNode == null) noodlesNode = new NoodlesNode();
        noodlesNode.noodleCount++;
    }

    public void Mix_1()
    {
        if (mix_1_Node == null) mix_1_Node = new Mix_1_Node();
        mix_1_Node.isMixed = true;
    }

    public void AddThickener()
    {
        if (thickenerNode == null) thickenerNode = new ThickenerNode();
        thickenerNode.thickenerCount++;
    }

    public void AddEgg()
    {
        if (eggNode == null) eggNode = new EggNode();
        eggNode.eggCount++;
    }


    public void Mix_2()
    {
        if (mix_2_Node == null) mix_2_Node = new Mix_2_Node();
        mix_2_Node.isMixed = true;
    }

    public void CreateWokGroup()
    {
        initWok();
        wokGroup.children = new List<OrderNode>
        {
            sauteeNode,
            noodlesNode,
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
            hitCollider.TryGetComponent(out PrepDish dish);

            CreateWokGroup();
            dish.potGroup = potGroup;
            dish.wokGroup = wokGroup;

            //Clear
            wokGroup = new WokGroup();
            potGroup = null;
            sauteeNode = null;
            noodlesNode = null;
            mix_1_Node = null;
            thickenerNode = null;
            eggNode = null;
            mix_2_Node = null;
            

            if (Debug.isDebugBuild) Debug.Log("Cleared WokNODE");

            revertDefaults();
            return;
        }

        revertDefaults();

    }

}