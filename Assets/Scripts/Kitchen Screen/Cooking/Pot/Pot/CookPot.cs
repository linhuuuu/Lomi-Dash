using PCG;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public KitchenDrag.Action lastAction;

    [SerializeField] private SpriteRenderer sink;
    [SerializeField] private SpriteRenderer[] woks;

    void Start()
    {

        promptSprite = new()
         {
             sink
         };

        foreach (SpriteRenderer wok in woks)
            promptSprite.Add(wok);

        animPot = GetComponent<AnimPot>();

        //Create Pot Node
        boilNode = new();
        seasoningNode = new();
        bonesNode = new();
        potGroup = new();

        potGroup.children = new List<OrderNode>
        {
            boilNode,
            bonesNode,
            seasoningNode
        };
    }

    #region AddNodes


    public IEnumerator AddWater()
    {
        if (boilNode == null) boilNode = new BoilNode();
        if (boilNode.count < maxCount)
        {
            boilNode.count = maxCount;
            UpdateBoilingState();

            lastAction = KitchenDrag.Action.WATER;
            KitchenDrag.Instance.SpecifyAction(lastAction);

            yield return animPot.AnimWater(hitCollider.gameObject);
        }
        else
        {
            AudioManager.instance.PlaySFX(SFX.MISTAKE);
            KitchenDrag.Instance.SpecifyMistake("Pot is Full!");
        }

        revertDefaults();
    }
    public void AddKnorr()
    {

        if (bonesNode?.count >= maxCount || bonesNode == null)
        {
            AudioManager.instance.PlaySFX(SFX.MISTAKE);
            KitchenDrag.Instance.SpecifyMistake("Enough Knorr cubes!");
            return;
        }

        if (boilNode?.count == 0 || boilNode == null)
        {
            AudioManager.instance.PlaySFX(SFX.MISTAKE);
            KitchenDrag.Instance.SpecifyMistake("Add Water First!");
            return;
        }

        if (bonesNode == null) bonesNode = new BonesNode();
        if (bonesNode.count < maxCount)
        {
            bonesNode.count++;
            UpdateBoilingState();

            lastAction = KitchenDrag.Action.KNORR;
            KitchenDrag.Instance.SpecifyAction(lastAction);

            animPot.OnAddKnorr();
        }
    }

    public void AddSeasoning(string type)
    {
        if (boilNode?.count == 0 || boilNode == null)
        {
            AudioManager.instance.PlaySFX(SFX.MISTAKE);
            KitchenDrag.Instance.SpecifyMistake("Add Water First!");
            return;
        }

        if (seasoningNode == null) seasoningNode = new SeasoningNode();

        switch (type)
        {
            case "Salt":
                if (seasoningNode.saltCount < maxCount * 2)
                {
                    seasoningNode.saltCount += maxCount * 2;
                    animPot.UpdateSeasoning();
                    lastAction = KitchenDrag.Action.SALT;
                    KitchenDrag.Instance.SpecifyAction(lastAction);
                }
                else
                {
                    AudioManager.instance.PlaySFX(SFX.MISTAKE);
                    KitchenDrag.Instance.SpecifyMistake("Enough salt!");
                }
                break;

            case "Pepper":
                if (seasoningNode.pepperCount < maxCount * 2)
                {
                    seasoningNode.pepperCount += maxCount * 2;
                    animPot.UpdateSeasoning();
                    lastAction = KitchenDrag.Action.PEPPER;
                    KitchenDrag.Instance.SpecifyAction(lastAction);
                }
                else
                {
                    AudioManager.instance.PlaySFX(SFX.MISTAKE);
                    KitchenDrag.Instance.SpecifyMistake("Enough Pepper!");
                }
                break;
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
                    boilingRoutine = StartCoroutine(BoilWater());

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

            if (boilNode.time == 1)
            {
                animPot.PlayBoilSFX(0);
                lastAction = KitchenDrag.Action.BOIL;
                KitchenDrag.Instance.SpecifyAction(lastAction);
            }

            else if (boilNode.time == 7)
                animPot.PlayBoilSFX(1);
            else if (boilNode.time == 15)
            {
                animPot.PlayBoilSFX(2);
                lastAction = KitchenDrag.Action.BOILED;
                KitchenDrag.Instance.SpecifyAction(lastAction);
            }
        }
    }
    #endregion
    #region Dropping
    public void OnMouseUp()
    {
        initDraggable();

        if (hitCollider == null)
        {
            if (Debug.isDebugBuild) Debug.Log("Pot hit nothing.");
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
            if (boilNode != null)
            {
                //Block Transfer if
                if (boilNode == null) { revertDefaults(); return; }
                if (boilNode.count == 0) { revertDefaults(); return; }
                if (targetWok.potGroup != null) { revertDefaults(); return; }

                //Transfer Pot Group
                PotGroup newPotGroup = new();
                int wokMaxCount = targetWok.maxCount;

                if (boilNode != null && boilNode.count > 0)
                    newPotGroup.children.Add(new BoilNode { count = Mathf.Min(boilNode.count, wokMaxCount), time = boilNode.time });
                else
                    newPotGroup.children.Add(new BoilNode());

                if (bonesNode != null && bonesNode.count > 0)
                    newPotGroup.children.Add(new BonesNode { count = Mathf.Min(boilNode.count, wokMaxCount) });
                else
                    newPotGroup.children.Add(new BonesNode());

                if (seasoningNode != null && seasoningNode.saltCount > 0 || seasoningNode.pepperCount > 0)
                    newPotGroup.children.Add(new SeasoningNode { pepperCount = Mathf.Min(seasoningNode.pepperCount, wokMaxCount * 2), saltCount = Mathf.Min(seasoningNode.saltCount, wokMaxCount * 2) });
                else
                    newPotGroup.children.Add(new SeasoningNode());

                targetWok.TransferPot(newPotGroup, animPot.GetBrothColor());

                lastAction = KitchenDrag.Action.TRANSFER;
                KitchenDrag.Instance.SpecifyAction(lastAction);

                //Reduce Count
                if (boilNode.count > 0)
                    boilNode.count -= wokMaxCount;

                if (bonesNode.count > 0)
                    bonesNode.count -= wokMaxCount;

                if (seasoningNode.saltCount > 0)
                    seasoningNode.saltCount -= wokMaxCount * 2;

                if (seasoningNode.pepperCount > 0)
                    seasoningNode.pepperCount -= wokMaxCount * 2;

                if (boilNode.count == 0)
                    boilNode.time = 0;

                //Anim Reduction
                animPot.OnReduceWater();
                if (Debug.isDebugBuild) Debug.Log("Cleared POTNODE");
            }

            revertDefaults();
            return;
        }

        if (hitCollider.tag == "Trash")
        {
            //Reduce Count
            if (boilNode.count > 0)
                boilNode.count = Mathf.Max(0, boilNode.count - maxCount);

            if (bonesNode.count > 0)
                bonesNode.count = Mathf.Max(0, bonesNode.count - maxCount);

            if (seasoningNode.saltCount > 0)
                seasoningNode.saltCount = Mathf.Max(0, seasoningNode.saltCount - maxCount);

            if (seasoningNode.pepperCount > 0)
                seasoningNode.pepperCount = Mathf.Max(0, seasoningNode.saltCount - maxCount);

            if (boilNode.count == 0)
                boilNode.time = 0;

            animPot.OnReduceWater();
            if (Debug.isDebugBuild) Debug.Log("Cleared All POTNODE");
        }

        revertDefaults();
    }
    #endregion
}