using PCG;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CookWok : DragAndDrop
{

    public bool stove_On = false;
    private SauteeNode sauteeNode;
    public PotNode potNode { get; set; }
    private NoodlesNode noodlesNode;
    private Mix_1_Node mix_1_Node;
    private ThickenerNode thickenerNode;
    private EggNode eggNode;
    private Mix_2_Node mix_2_Node;
    private WokNode wokNode;
    private Coroutine cookingRoutine;


    void Awake()
    {
        if (wokNode == null) wokNode = new WokNode("WOK");
    }
    public void SauteePan(string type)
    {
        if (sauteeNode == null) sauteeNode = new SauteeNode("SAUTEE");
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
        if (noodlesNode == null) noodlesNode = new NoodlesNode("NOODLES");

        noodlesNode.noodleCount++;
    }

    public void Mix_1()
    {
        if (mix_1_Node == null) mix_1_Node = new Mix_1_Node("MIX_1");
        mix_1_Node.isMixed = true;
    }

    public void ThickenerNode()
    {
        if (thickenerNode == null) thickenerNode = new ThickenerNode("THICKENER");
        thickenerNode.thickenerCount++;
    }

    public void AddEgg()
    {
        if (eggNode == null) eggNode = new EggNode("EGG");
        eggNode.eggCount++;
    }

    public void Mix_2()
    {
        if (mix_2_Node == null) mix_2_Node = new Mix_2_Node("MIX_2");
        mix_2_Node.isMixed = true;
    }

    public void CreateWokNode()
    {
        if (wokNode == null) wokNode = new WokNode("WOK");
        if (sauteeNode == null) sauteeNode = new SauteeNode("SAUTEE");
        if (noodlesNode == null) noodlesNode = new NoodlesNode("NOODLES");
        if (mix_1_Node == null) mix_1_Node = new Mix_1_Node("MIX_1");
        if (thickenerNode == null) thickenerNode = new ThickenerNode("THICKENER");
        if (eggNode == null) eggNode = new EggNode("THICKENER");
        if (mix_2_Node == null) mix_2_Node = new Mix_2_Node("MIX_2");

        wokNode.children = new List<OrderNode>
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

        if (hitCollider.TryGetComponent(out Sink targetSink))
        {

            revertDefaults();
            return;
        }

        if (hitCollider.TryGetComponent(out PrepDish dish))
        {
            // CreatePotNode();
            

            //Simplified Reset, Does not account for large Bowls;
            potNode = null;
            sauteeNode = null;
            noodlesNode = null;
            mix_1_Node = null;
            thickenerNode = null;
            eggNode = null;
            mix_2_Node = null;

            if (Debug.isDebugBuild) Debug.Log("Cleared POTNODE");

            revertDefaults();
            return;
        }
        revertDefaults();
    }

}