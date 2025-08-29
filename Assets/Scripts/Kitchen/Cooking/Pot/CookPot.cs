using PCG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CookPot : DragAndDrop
{
    public bool stove_1_On = false;
    public BoilNode boilNode { set; get; }
    public SeasoningPotNode seasoningPotNode { set; get; }
    public BonesNode bonesNode { set; get; }
    public PotNode potNode { set; get;}


    private Coroutine boilingRoutine;
    public void AddWater()
    {
        if (boilNode == null) boilNode = new BoilNode("BOIL");

        if (boilNode.waterHeld < 2)
        {
            boilNode.waterHeld++;
            if (Debug.isDebugBuild) Debug.Log("Added Water x" + boilNode.waterHeld);

            //Start Boiling
            UpdateBoilingState();
        }
    }

    public void AddBones()  //if theres time add uses meter to the bones used or remove the white scum floating...
    {
        if (bonesNode == null) bonesNode = new BonesNode("BONES");

        if (bonesNode.count < 2)
        {
            bonesNode.count++;
            if (Debug.isDebugBuild) Debug.Log("Added Bones x" + bonesNode.count);
        }
        UpdateBoilingState();
    }

    public void AddSeasoning(string type)
    {
        if (potNode == null) potNode = new PotNode("POT_NODE");
        if (seasoningPotNode == null) seasoningPotNode = new SeasoningPotNode("SEASONING_POT");

        switch (type)
        {
            case "Salt":
                seasoningPotNode.saltCount++; break;
            case "Pepper":
                seasoningPotNode.pepperCount++; break;
            case "Bawang":
                seasoningPotNode.bawangCount++; break;
            default:
                if (Debug.isDebugBuild) Debug.Log("Walang seasoning");
                type = "null";
                break;
        }
        if (Debug.isDebugBuild) Debug.Log("Added Seasoning of Type " + type);
    }

    public void ToggleStove()
    {
        stove_1_On = !stove_1_On;
        if (Debug.isDebugBuild) Debug.Log("Stove is " + stove_1_On);

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
            if (Debug.isDebugBuild) Debug.Log("Completed Boil at " + boilNode.time + "!");
            boilingRoutine = null;
        }
    }

    private IEnumerator BoilWater()
    {
        while (stove_1_On && boilNode != null && boilNode.time < 15)
        {
            yield return new WaitForSeconds(1);

            if (boilNode.waterHeld > 0 && bonesNode != null)
            {
                boilNode.time++;
                if (Debug.isDebugBuild) Debug.Log("Boiled Water For " + boilNode.time + " Seconds.");
            }
            else
            {
                if (Debug.isDebugBuild) Debug.Log("No water or Bones! ");
                break;
            }
        }

        boilingRoutine = null;
    }

    public void CreatePotNode()
    {
        if (boilNode == null) boilNode = new BoilNode("BOIL");
        if (bonesNode == null) bonesNode = new BonesNode("BONES");
        if (seasoningPotNode == null) seasoningPotNode = new SeasoningPotNode("SEASONING_POT");

        // Create the container node
        potNode = new PotNode("POT_NODE");
        potNode.children = new List<OrderNode>
        {
            // Add children
            boilNode,
            bonesNode,
            seasoningPotNode
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
            AddWater();
            revertDefaults();
            return;
        }

        if (hitCollider.TryGetComponent(out CookWok targetWok))
        {
            CreatePotNode();
            targetWok.potNode = potNode;
            //Simplified Reset, Does not account for large Bowls;
            potNode = null;
            boilNode = null;
            bonesNode = null;
            seasoningPotNode = null;

            if (Debug.isDebugBuild) Debug.Log("Cleared POTNODE");
            if (Debug.isDebugBuild) Debug.Log(targetWok.potNode.id);

            revertDefaults();
            return;
        }
        revertDefaults();
    }
}