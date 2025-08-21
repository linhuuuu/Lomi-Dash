using PCG;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CookWok : MonoBehaviour, IDropHandler
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
        if(wokNode == null) wokNode = new WokNode("WOK");
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

    public void EggNode()
    {
        if (eggNode == null) eggNode = new EggNode("THICKENER");
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

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent<CookWok>(out CookWok targetWok))
        {
            //Creates the PotNode and Passes it to the wok.
            CreateWokNode();
            targetWok.potNode = potNode;

            //Simplified Reset, Does not account for large Bowls;
            potNode = null;
            if (Debug.isDebugBuild) Debug.Log("Cleared WOKNODE");
        }
    }
}