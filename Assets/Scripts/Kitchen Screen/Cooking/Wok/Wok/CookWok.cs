using PCG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookWok : DragAndDrop
{
    public SauteeNode sauteeNode { private set; get; }
    public NoodlesNode noodlesNode { private set; get; }
    public SoySauceNode soySauceNode { private set; get; }
    public EggNode eggNode { private set; get; }
    public ThickenerNode thickenerNode { private set; get; }

    public PotGroup potGroup { set; get; }
    public WokGroup wokGroup { private set; get; }
    public AnimWok animWok { private set; get; }
    public bool tutorialTransfer { set; get; } = false;

    private Coroutine sauteeRoutine;
    private Coroutine noodlesRoutine;

    public int maxCount { set; get; } = 1;
    public bool stove_On { private set; get; } = false;

    public KitchenDrag.Action lastAction;

    void Start()
    {
        promptSprite = new();
                foreach (SpriteRenderer dish in KitchenDrag.Instance.dishBlankets)
            promptSprite.Add(dish);

        InitWok();
        animWok = GetComponent<AnimWok>();
    }

    private void InitWok()
    {
        if (wokGroup == null) wokGroup = new WokGroup();
        if (sauteeNode == null) sauteeNode = new SauteeNode();
        if (noodlesNode == null) noodlesNode = new NoodlesNode();
        if (soySauceNode == null) soySauceNode = new SoySauceNode();
        if (thickenerNode == null) thickenerNode = new ThickenerNode();
        if (eggNode == null) eggNode = new EggNode();
        //potGroup = null;

        wokGroup.children = new List<OrderNode>
        {
            sauteeNode,
            noodlesNode,
            soySauceNode,
            thickenerNode,
            eggNode,
        };
    }

    #region CookingRoutine
    public void ToggleStove()
    {
        stove_On = !stove_On;

        if (!stove_On)
            animWok.StopJitter();
        else
            animWok.StartJitter();

        UpdateSauteeState();
        UpdateNoodleState();
    }

    private void UpdateSauteeState()
    {
        if (sauteeNode != null &&
            (sauteeNode.onionTime < 15 || sauteeNode.oilTime < 15 || sauteeNode.bawangTime < 15))
        {
            if (stove_On)
            {
                if (sauteeRoutine == null)
                    sauteeRoutine = StartCoroutine(OnSautee());
            }
            else if (!stove_On && sauteeRoutine != null)
            {
                StopCoroutine(sauteeRoutine);
                sauteeRoutine = null;
            }
        }
        else if (!stove_On && sauteeRoutine != null) // Safety
        {
            StopCoroutine(sauteeRoutine);
            sauteeRoutine = null;
        }
    }

    private IEnumerator OnSautee()
    {
        while (stove_On && sauteeNode != null && (sauteeNode.onionTime < 15 || sauteeNode.oilTime < 15 || sauteeNode.bawangTime < 15))
        {
            yield return new WaitForSeconds(1);
            if (sauteeNode.onionCount > 0 && sauteeNode.onionTime < 15)
                sauteeNode.onionTime++;

            if (sauteeNode.bawangCount > 0 && sauteeNode.bawangTime < 15)
                sauteeNode.bawangTime++;

            if (sauteeNode.oilCount > 0 && sauteeNode.oilTime < 15)
                sauteeNode.oilTime++;

            if (sauteeNode.oilTime == 7)
                animWok.IngredientChangeState("Oil", 0);
            if (sauteeNode.oilTime == 15)
                animWok.IngredientChangeState("Oil", 1);

            if (sauteeNode.onionTime == 7)
                animWok.IngredientChangeState("Onion", 0);
            if (sauteeNode.onionTime == 15)
                animWok.IngredientChangeState("Onion", 1);

            if (sauteeNode.bawangTime == 7)
                animWok.IngredientChangeState("Bawang", 0);
            if (sauteeNode.bawangTime == 15)
                animWok.IngredientChangeState("Bawang", 1);

            if (sauteeNode.oilTime == 7 || sauteeNode.onionTime == 7 || sauteeNode.bawangTime == 7)
                animWok.PlaceSizzleSFX(2);

            if (sauteeNode.oilTime == 15 && sauteeNode.onionTime == 15 && sauteeNode.bawangTime == 15)
                animWok.PlaceSizzleSFX(3);
        }
    }

    private void UpdateNoodleState()
    {
        if (noodlesNode != null && noodlesNode.time < 15)
        {
            if (stove_On)
            {
                if (noodlesRoutine == null)
                    noodlesRoutine = StartCoroutine(OnNoodles());
            }
            else if (!stove_On && noodlesRoutine != null)
            {
                StopCoroutine(noodlesRoutine);
                noodlesRoutine = null;
            }
        }
        else if (!stove_On && noodlesRoutine != null)
        {
            StopCoroutine(noodlesRoutine);
            noodlesRoutine = null;
        }
    }


    private IEnumerator OnNoodles()
    {
        while (stove_On && noodlesNode != null && noodlesNode.time < 15)
        {
            yield return new WaitForSeconds(1);
            if (noodlesNode.count > 0 && noodlesNode.time < 15)
                noodlesNode.time++;

            if (noodlesNode.time == 7)
            {
                animWok.IngredientChangeState("Noodles", 0);
                animWok.PlaceSizzleSFX(2);
            }

            if (noodlesNode.time == 15)
            {
                animWok.IngredientChangeState("Noodles", 1);
                animWok.PlaceSizzleSFX(2);
            }

        }
    }

    #endregion
    #region Wok Actions

    public void SauteePan(string type)
    {
        if (sauteeNode == null) sauteeNode = new SauteeNode();

        switch (type)
        {
            case "Oil":
                if (sauteeNode.oilCount == maxCount)
                {
                    AudioManager.instance.PlaySFX(SFX.MISTAKE);
                    KitchenDrag.Instance.SpecifyMistake($"Enough Oil!");
                    return;
                }
                else
                {
                    sauteeNode.oilCount += maxCount;
                    animWok.PlaceIngredient("Oil");

                    lastAction = KitchenDrag.Action.OIL;
                    KitchenDrag.Instance.SpecifyAction(lastAction);
                }
                break;
            case "Bawang":
                if (sauteeNode.bawangCount == maxCount)
                {
                    AudioManager.instance.PlaySFX(SFX.MISTAKE);
                    KitchenDrag.Instance.SpecifyMistake($"Enough Bawang!");
                    return;
                }
                else
                {
                    sauteeNode.bawangCount += maxCount;
                    animWok.PlaceIngredient("Bawang");

                    lastAction = KitchenDrag.Action.BAWANG;
                    KitchenDrag.Instance.SpecifyAction(lastAction);
                }
                break;
            case "Onion":
                if (sauteeNode.onionCount == maxCount)
                {
                    AudioManager.instance.PlaySFX(SFX.MISTAKE);
                    KitchenDrag.Instance.SpecifyMistake($"Enough Onion!");
                    return;
                }
                else
                {
                    sauteeNode.onionCount += maxCount;
                    animWok.PlaceIngredient("Onion");

                    lastAction = KitchenDrag.Action.ONION;
                    KitchenDrag.Instance.SpecifyAction(lastAction);
                }
                break;
            default:
                if (Debug.isDebugBuild) Debug.Log("Unrecognized Type."); break;
        }
        UpdateSauteeState();
    }

    public void AddSoySauce()
    {
        if (soySauceNode == null) soySauceNode = new SoySauceNode();
        if (soySauceNode.count == maxCount)
        {
            AudioManager.instance.PlaySFX(SFX.MISTAKE);
            KitchenDrag.Instance.SpecifyMistake($"Enough Soy Sauce!");
            return;
        }
        else
        {
            soySauceNode.count += maxCount;
            StartCoroutine(animWok.AddSoySauce());

            lastAction = KitchenDrag.Action.SOY_SAUCE;
            KitchenDrag.Instance.SpecifyAction(lastAction);
        }


    }

    public void AddNoodles()
    {
        if (noodlesNode == null) noodlesNode = new NoodlesNode();

        if (noodlesNode.count == maxCount)
        {
            AudioManager.instance.PlaySFX(SFX.MISTAKE);
            KitchenDrag.Instance.SpecifyMistake($"Enough Noodles!");
            return;
        }
        else
        {
            noodlesNode.count += maxCount;

            animWok.PlaceIngredient("Noodles");
            UpdateNoodleState();

            lastAction = KitchenDrag.Action.NOODLES;
            KitchenDrag.Instance.SpecifyAction(lastAction);
        }

    }

    public void TransferPot(PotGroup potGroup, Color color)
    {
        if (this.potGroup != null) return;

        this.potGroup = potGroup;
        animWok.AnimPotGroup(color);
    }

    public void AddThickener()
    {
        if (thickenerNode == null) thickenerNode = new ThickenerNode();

        if (thickenerNode.count == maxCount)
        {
            AudioManager.instance.PlaySFX(SFX.MISTAKE);
            KitchenDrag.Instance.SpecifyMistake($"Enough Cassava Flour!");
            return;
        }
        else
        {
            thickenerNode.count += maxCount;
            animWok.PlaceSlurry("Thickener");

            lastAction = KitchenDrag.Action.CASSAVA;
            KitchenDrag.Instance.SpecifyAction(lastAction);
        }
    }

    public void AddEgg()
    {
        if (eggNode == null) eggNode = new EggNode();

        if (eggNode.count == maxCount)
        {
            AudioManager.instance.PlaySFX(SFX.MISTAKE);
            KitchenDrag.Instance.SpecifyMistake($"Enough Egg!");
            return;
        }
        else
        {
            eggNode.count += maxCount;
            animWok.PlaceSlurry("Egg");

            lastAction = KitchenDrag.Action.EGG;
            KitchenDrag.Instance.SpecifyAction(lastAction);
        }

    }

    public void OnMix()
    {
        animWok.MixIngredients();

        if ((eggNode != null && eggNode.count > 0 && eggNode.isMixed == false) || (thickenerNode != null && thickenerNode.count > 0 && thickenerNode.isMixed == false))
        {
            if (eggNode.count > 0 && eggNode.isMixed == false)
            {
                lastAction = KitchenDrag.Action.MIXED;
                KitchenDrag.Instance.SpecifyAction(lastAction);
                eggNode.isMixed = true;
            }

            if (thickenerNode.count > 0 && thickenerNode.isMixed == false)
            {
                lastAction = KitchenDrag.Action.MIXED;
                KitchenDrag.Instance.SpecifyAction(lastAction);
                thickenerNode.isMixed = true;
            }

            StartCoroutine(animWok.MixSlurry());
        }
    }

    #endregion
    #region onDrop

    public void OnMouseUp()
    {
        initDraggable();

        if (hitCollider == null)
        {
            if (Debug.isDebugBuild) Debug.Log("Wok Got Nothing");
            animWok.PlayReturnToStoveSFX();
            revertDefaults();
            return;
        }

        if (hitCollider.tag == "Dish")
        {
            if (!hitCollider.TryGetComponent(out PrepDish dish)) { revertDefaults(); return; }
            if (dish.wokGroup != null) { revertDefaults(); return; }

            int dishMaxCount = dish.isLarge ? 2 : 1;

            // Create new WokGroup for transfer
            var transferGroup = new WokGroup();
            var children = new List<OrderNode>();

            // Transfer Sautee
            if (sauteeNode != null && (sauteeNode.oilCount > 0 || sauteeNode.onionCount > 0 || sauteeNode.bawangCount > 0))
            {
                var _sautee = new SauteeNode
                {
                    oilCount = Mathf.Min(sauteeNode.oilCount, dishMaxCount),
                    oilTime = sauteeNode.oilTime,
                    onionCount = Mathf.Min(sauteeNode.onionCount, dishMaxCount),
                    onionTime = sauteeNode.onionTime,
                    bawangCount = Mathf.Min(sauteeNode.bawangCount, dishMaxCount),
                    bawangTime = sauteeNode.bawangTime
                };
                children.Add(_sautee);
            }

            // Transfer Noodles
            if (noodlesNode != null && noodlesNode.count > 0)
            {
                var _noodles = new NoodlesNode
                {
                    count = Mathf.Min(noodlesNode.count, dishMaxCount),
                    time = noodlesNode.time
                };
                children.Add(_noodles);
            }

            // Transfer Soy Sauce
            if (soySauceNode != null && soySauceNode.count > 0)
            {
                var _soy = new SoySauceNode { count = Mathf.Min(soySauceNode.count, dishMaxCount) };
                children.Add(_soy);
            }

            // Transfer Thickener
            if (thickenerNode != null && thickenerNode.count > 0)
            {
                var _thick = new ThickenerNode
                {
                    count = Mathf.Min(thickenerNode.count, dishMaxCount),
                    isMixed = thickenerNode.isMixed,
                };
                children.Add(_thick);
            }

            // Transfer Egg
            if (eggNode != null && eggNode.count > 0)
            {
                var _egg = new EggNode
                {
                    count = Mathf.Min(eggNode.count, dishMaxCount),
                    isMixed = eggNode.isMixed,
                };
                children.Add(_egg);
            }

            transferGroup.children = children;
            dish.potGroup = potGroup;
            dish.wokGroup = transferGroup;

            // Anim Visuals
            animWok.TransferWok(dish);

            // Reduce local counts
            if (sauteeNode != null)
            {
                sauteeNode.oilCount = Mathf.Max(0, sauteeNode.oilCount - dishMaxCount);
                sauteeNode.onionCount = Mathf.Max(0, sauteeNode.onionCount - dishMaxCount);
                sauteeNode.bawangCount = Mathf.Max(0, sauteeNode.bawangCount - dishMaxCount);
            }

            if (noodlesNode != null)
                noodlesNode.count = Mathf.Max(0, noodlesNode.count - dishMaxCount);

            if (soySauceNode != null)
                soySauceNode.count = Mathf.Max(0, soySauceNode.count - dishMaxCount);

            if (thickenerNode != null)
                thickenerNode.count = Mathf.Max(0, thickenerNode.count - dishMaxCount);

            if (eggNode != null)
                eggNode.count = Mathf.Max(0, eggNode.count - dishMaxCount);

            // Check if all ingredients are now zero
            bool hasRemaining =
                (sauteeNode?.oilCount ?? 0) > 0 ||
                (sauteeNode?.onionCount ?? 0) > 0 ||
                (sauteeNode?.bawangCount ?? 0) > 0 ||
                (noodlesNode?.count ?? 0) > 0 ||
                (soySauceNode?.count ?? 0) > 0 ||
                (thickenerNode?.count ?? 0) > 0 ||
                (eggNode?.count ?? 0) > 0;

            if (hasRemaining)
            {
                animWok.ReduceWokCount();
                if (Debug.isDebugBuild) Debug.Log("Cleared 1 instance of WokNODE");
            }
            else
            {
                // Reset state completely
                sauteeNode.oilTime = 0;
                sauteeNode.onionTime = 0;
                sauteeNode.bawangTime = 0;
                noodlesNode.time = 0;
                thickenerNode.isMixed = false;
                eggNode.isMixed = false;
                potGroup = null;

                // Reset visuals
                animWok.ResetStates();
                sauteeRoutine = null;
                noodlesRoutine = null;

                if (Debug.isDebugBuild) Debug.Log("Cleared ALL instances of WokNODE");
            }
            animWok.PlayTransferToDishSFX();

            if (GameManager.instance.state == GameManager.gameState.tutorial)
            {
                tutorialTransfer = true;
                TutorialManager.instance.dish = dish;
            }


            revertDefaults();
            return;
        }

        if (hitCollider.tag == "Trash")
        {

            // Reset state completely
            sauteeNode.oilCount = 0;
            sauteeNode.onionCount = 0;
            sauteeNode.bawangCount = 0;
            noodlesNode.count = 0;
            soySauceNode.count = 0;
            thickenerNode.count = 0;
            eggNode.count = 0;
            sauteeNode.oilTime = 0;
            sauteeNode.onionTime = 0;
            sauteeNode.bawangTime = 0;
            noodlesNode.time = 0;
            thickenerNode.isMixed = false;
            eggNode.isMixed = false;
            potGroup = null;

            // Reset visuals
            animWok.ResetStates();
            sauteeRoutine = null;
            noodlesRoutine = null;

            revertDefaults();
            return;
        }
        revertDefaults();
        return;
    }
    #endregion
}