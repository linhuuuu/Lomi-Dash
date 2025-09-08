using PCG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CookPot : DragAndDrop
{
    public bool stove_On = false;
    public BoilNode boilNode { private set; get; }
    public SeasoningNode seasoningNode { private set; get; }
    public BonesNode bonesNode { private set; get; }
    public PotGroup potGroup { private set; get; }
    private Coroutine boilingRoutine;

    private void InitPot()
    {
        if (boilNode == null) boilNode = new BoilNode();
        if (bonesNode == null) bonesNode = new BonesNode();
        if (potGroup == null) potGroup = new PotGroup();
        if (seasoningNode == null) seasoningNode = new SeasoningNode();
    }
    public void AddWater()
    {
        if (boilNode == null) boilNode = new BoilNode();
        if (boilNode.waterHeld < 2)
        {
            boilNode.waterHeld++;
            UpdateBoilingState();
        }
    }
    public void AddBones()  //if theres time add uses meter to the bones used or remove the white scum floating...
    {
        if (bonesNode == null) bonesNode = new BonesNode();
        if (bonesNode.count < 2)
        {
            bonesNode.count++;
            UpdateBoilingState();
        }
    }
    public void AddSeasoning(string type)
    {
        if (seasoningNode == null) seasoningNode = new SeasoningNode();
        switch (type)
        {
            case "Salt":
                seasoningNode.saltCount++; break;
            case "Pepper":
                seasoningNode.pepperCount++; break;
            case "Bawang":
                seasoningNode.bawangCount++; break;
            default:
                break;
        }
    }
    public void ToggleStove()
    {
        stove_On = !stove_On;
        UpdateBoilingState();
    }

    private void UpdateBoilingState()
    {
        if (stove_On && boilNode != null && boilNode.waterHeld > 0)
        {
            if (boilingRoutine == null)
            {
                boilingRoutine = StartCoroutine(BoilWater());
            }
        }

        else if (!stove_On && boilingRoutine != null)
        {
            StopCoroutine(boilingRoutine);
            boilingRoutine = null;
        }
    }

    private IEnumerator BoilWater()
    {
        while (stove_On && boilNode != null && boilNode.time < 15)
        {
            yield return new WaitForSeconds(1);

            if (boilNode.waterHeld > 0 && bonesNode != null)
                boilNode.time++;
        }
        boilingRoutine = null;
    }

    public void CreatePotNode()
    {
        InitPot();
        potGroup.children = new List<OrderNode>
        {
            boilNode,
            bonesNode,
            seasoningNode
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

        if (hitCollider.tag == "Sink")
        {
            AddWater();
            revertDefaults();
            return;
        }

        if (hitCollider.TryGetComponent(out CookWok targetWok))
        {
            if (targetWok.mix_1_Node == null)
            {
                CreatePotNode();
                targetWok.potGroup = potGroup;

                //Simplified Reset, Does not account for large Bowls;
                potGroup = null;
                boilNode = null;
                bonesNode = null;
                seasoningNode = null;

                if (Debug.isDebugBuild) Debug.Log("Cleared POTNODE");
            }
            else
            {
                if (Debug.isDebugBuild) Debug.Log("PotNode Transferrence Failed.");
            }

            revertDefaults();
            return;
        }

        revertDefaults();
    }
}