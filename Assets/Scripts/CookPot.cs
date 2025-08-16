using PCG;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CookPot : MonoBehaviour
{
    public bool stove_1_On = false;
    public BoilNode boilNode;
    public SeasoningPotNode seasoningPotNode;
    public BonesNode bonesNode;
    public OrderNode potNode;
   

    private Coroutine boilingRoutine;
   
    public void CreateBoilNode()
    {
        boilNode = new BoilNode();
        boilNode.id = "BOIL_TIME";
        boilNode.waterHeld = 0;
        boilNode.time = 0;
    }

    public void CreateBonesNode()
    {
        bonesNode = new BonesNode();
        boilNode.id = "BONES";
    }

    public void CreateSeasoningPotNode()
    {
        seasoningPotNode = new SeasoningPotNode();
        seasoningPotNode.id = "DEASONING_POT_NODE";
    }

    public void AddWater()
    {
        if (boilNode == null) CreateBoilNode();

        if (boilNode.waterHeld < 2)
        {
            boilNode.waterHeld++;
            Debug.Log("Added Water x" + boilNode.waterHeld);

           //Start Boiling
            UpdateBoilingState();
        }
    }

    public void AddBones()
    {
        if (bonesNode == null) CreateBonesNode();

        if (bonesNode.count < 2)
        {
            bonesNode.count++;
            Debug.Log("Added Bones x" + bonesNode.count);
        }
    }

    public void AddSeasoning(string type)
    {
        if (seasoningPotNode == null) CreateSeasoningPotNode();

        switch (type)
        {
            case "Salt":
                seasoningPotNode.saltCount++; break;
            case "Pepper":
                 seasoningPotNode.pepperCount++; break;
        }
    }

    public void ToggleStove1()
    {
        stove_1_On = !stove_1_On;
        Debug.Log("Stove is " + stove_1_On);

        // Always update boiling state
        UpdateBoilingState();
    }

    private void UpdateBoilingState()
    {
        // If stove is on and we have water → start/continue boiling
        if (stove_1_On && boilNode != null && boilNode.waterHeld > 0)
        {
            if (boilingRoutine == null)
            {
                boilingRoutine = StartCoroutine(BoilWater());
            }
        }
        // If stove is off → stop boiling
        else if (!stove_1_On && boilingRoutine != null)
        {
            StopCoroutine(boilingRoutine);
            Debug.Log("Completed Boil at " + boilNode.time + "!");
            boilingRoutine = null;
        }
    }

    private IEnumerator BoilWater()
    {
        while (stove_1_On && boilNode != null && boilNode.time < 15)
        {
            yield return new WaitForSeconds(1);

            if (boilNode.waterHeld > 0)
            {
                boilNode.time++;
                Debug.Log("Boiled Water For " + boilNode.time + " Seconds.");
            }
            else
            {
                Debug.Log("No water! Stopping boil.");
                break;
            }
        }

        boilingRoutine = null;
    }

    public void CreatePotNode()
    {
        // Ensure boilNode and bonesNode exist
        if (boilNode == null)
            CreateBoilNode(); // Assumes this creates and assigns boilNode

        if (bonesNode == null)
            CreateBonesNode(); // Assumes this creates and assigns bonesNode

        // Create the container node
        potNode = new OrderNode();
        potNode.id = "POT";
        potNode.children = new List<OrderNode>(); // Ensure children list exists

        // Add children
        potNode.children.Add(boilNode);
        potNode.children.Add(bonesNode);

        // Print the full structure
        Debug.Log("=== Pot Node Created ===");
        PrintNode(potNode, "");
        Debug.Log("========================");
    }

    //DEBUG
    private void PrintNode(OrderNode node, string indent)
    {
        // Print current node
        string nodeInfo = node switch
        {
           BoilNode b => $"[BoilNode] Water: {b.waterHeld}, Time: {b.time}s",
           BonesNode bn => $"[BooleanNode] {bn.id} = {bn.count}",
            _ => $"[{node.GetType().Name}] {node.id}"
        };

        Debug.Log($"{indent}├─ {nodeInfo}");

        // Recurse into children
        for (int i = 0; i < node.children.Count; i++)
        {
            string newIndent = indent + "│  ";
            if (i == node.children.Count - 1)
                newIndent = indent + "   ";

            PrintNode(node.children[i], newIndent);
        }
    }
    public void PourToPan()
    {
        CreatePotNode();

        //Simplified Reset, Does not account large Bowls;
        potNode = null;
        boilNode = null;
        bonesNode = null;
        Debug.Log("Cleared POTNODE");
    }

}