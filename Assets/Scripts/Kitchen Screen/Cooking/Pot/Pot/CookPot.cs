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

    public AnimPot animPot { private set; get; }
    private Coroutine boilingRoutine;

    public int maxCount { set; get; } = 2;

    void Start()
    {
        animPot = GetComponent<AnimPot>();
    }

    private void InitPot()
    {
        if (boilNode == null) boilNode = new BoilNode();
        if (bonesNode == null) bonesNode = new BonesNode();
        if (potGroup == null) potGroup = new PotGroup();
        if (seasoningNode == null) seasoningNode = new SeasoningNode();
    }

    #region AddNodes
    public IEnumerator AddWater()
    {
        if (boilNode == null) boilNode = new BoilNode();
        if (boilNode.count < maxCount)
        {
            boilNode.count = maxCount;
            UpdateBoilingState();

            //Anim
            yield return animPot.AnimWater(hitCollider.gameObject);
        }

        revertDefaults();
    }
    public void AddKnorr()
    {
        if (boilNode == null)
        {
            if (Debug.isDebugBuild) Debug.Log("No BoilNode Yet!");
            return;
        }

        if (bonesNode == null) bonesNode = new BonesNode();
        if (bonesNode.count < maxCount)//maxPot
        {
            bonesNode.count++;
            UpdateBoilingState();
        }

        animPot.OnAddKnorr();
    }

    public void AddSeasoning(string type)
    {
        if (boilNode == null)
        {
            if (Debug.isDebugBuild) Debug.Log("No BoilNode Yet!");
            return;
        }

        if (seasoningNode == null) seasoningNode = new SeasoningNode();
        switch (type)
        {
            case "Salt":
                if (seasoningNode.saltCount < maxCount)
                    seasoningNode.saltCount++; break;
            case "Pepper":
                if (seasoningNode.pepperCount < maxCount)
                    seasoningNode.pepperCount++; break;
            default:
                break;
        }
    }

    #endregion
    #region Boiling

    public void ToggleStove()
    {
        stove_On = !stove_On;
        UpdateBoilingState();
    }

    private void UpdateBoilingState()
    {
        if (boilNode != null && boilNode.count > 0)
            if (stove_On)
            {
                if (boilingRoutine == null)
                {
                    boilingRoutine = StartCoroutine(BoilWater());
                }
            }

            else if (!stove_On && boilingRoutine != null)
            {
                animPot.StopBoil();
                StopCoroutine(boilingRoutine);
                boilingRoutine = null;
            }
    }

    private IEnumerator BoilWater()
    {
        animPot.OnBoil();
        while (stove_On && boilNode != null && boilNode.time < 15)
        {
            yield return new WaitForSeconds(1);

            if (boilNode.count > 0 && bonesNode != null)
                boilNode.time++;

        }
        boilingRoutine = null;
    }

    #endregion

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
            if (targetWok.mix_1_Node == null && boilNode != null)
            {
                //Block Transfer if
                if (boilNode == null) { revertDefaults(); return; }
                if (boilNode.count == 0) { revertDefaults(); return; }

                if (targetWok.potGroup != null) { revertDefaults(); return; }
                if (targetWok?.noodlesNode != null && targetWok.noodlesNode.count != 0) { revertDefaults(); return; }


                //Create Pot Group
                CreatePotNode();
                targetWok.potGroup = potGroup;

                //Anim Wok
                targetWok.animWok.ToggleBroth(true);
                targetWok.animWok.ToggleSwirl(true);


                //Reduce Count
                if (boilNode.count > 0)
                    boilNode.count--;

                if (bonesNode.count > 0)
                    bonesNode.count--;

                if (seasoningNode.saltCount > 0)
                    seasoningNode.saltCount--;

                if (seasoningNode.pepperCount > 0)
                    seasoningNode.pepperCount--;

                //Anim Reduction
                animPot.OnReduceWater();
                if (Debug.isDebugBuild) Debug.Log("Cleared POTNODE");
            }

            revertDefaults();
            return;
        }
        revertDefaults();
    }
}