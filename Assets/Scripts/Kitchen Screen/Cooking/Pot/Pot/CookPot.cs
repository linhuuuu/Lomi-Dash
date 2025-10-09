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
    public AnimPot animPot { private set; get; }

    void Start()
    {
        animPot = GetComponent<AnimPot>();
        animPot.pot = this;
    }

    private void InitPot()
    {
        if (boilNode == null) boilNode = new BoilNode();
        if (bonesNode == null) bonesNode = new BonesNode();
        if (potGroup == null) potGroup = new PotGroup();
        if (seasoningNode == null) seasoningNode = new SeasoningNode();
    }
    public IEnumerator AddWater()
    {
        if (boilNode == null) boilNode = new BoilNode();
        if (boilNode.count < 2)
        {
            UpdateBoilingState();
            yield return animPot.AnimWater(hitCollider.gameObject);

            boilNode.count++;
        }

        revertDefaults();
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
        if (boilNode != null && boilNode.count > 0)
            animPot.ChangeToBrothColor();

        if (stove_On)
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

            if (boilNode.count > 0 && bonesNode != null)
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
            StartCoroutine(AddWater());
            return;
        }

        if (hitCollider.TryGetComponent(out CookWok targetWok))
        {
            if (targetWok.mix_1_Node == null && boilNode != null)   //must have water before transferring
            {
                //Create and Transfer
                CreatePotNode();
                targetWok.potGroup = potGroup;

                // //Anim
                // targetWok.animWok.ToggleBroth();
                // targetWok.animWok.ToggleSwirl();
                
                animPot.ClearPot();

                //Simplified Reset, Does not account for large Bowls;
                potGroup = null;
                boilNode = null;
                bonesNode = null;
                seasoningNode = null;

                if (Debug.isDebugBuild) Debug.Log("Cleared POTNODE");
            }

            revertDefaults();
            return;
        }



        revertDefaults();
    }
}