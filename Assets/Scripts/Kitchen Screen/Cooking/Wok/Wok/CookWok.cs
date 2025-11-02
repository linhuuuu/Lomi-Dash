using PCG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookWok : DragAndDrop
{
    public SauteeNode sauteeNode { private set; get; }
    public NoodlesNode noodlesNode { private set; get; }
    public SoySauceNode soysauceNode { private set; get; }
    public EggNode eggNode { private set; get; }
    public ThickenerNode thickenerNode { private set; get; }

    public PotGroup potGroup { set; get; }
    public WokGroup wokGroup { private set; get; }
    public AnimWok animWok { private set; get; }

    private Coroutine sauteeRoutine;
    private Coroutine noodlesRoutine;

    public int maxCount { set; get; } = 1;
    public bool stove_On { private set; get; } = false;

    void Start()
    {
        InitWok();
        animWok = GetComponent<AnimWok>();
    }

    private void InitWok()
    {
        if (wokGroup == null) wokGroup = new WokGroup();
        if (sauteeNode == null) sauteeNode = new SauteeNode();
        if (noodlesNode == null) noodlesNode = new NoodlesNode();
        if (soysauceNode == null) soysauceNode = new SoySauceNode();
        if (thickenerNode == null) thickenerNode = new ThickenerNode();
        if (eggNode == null) eggNode = new EggNode();
        //potGroup = null;

        wokGroup.children = new List<OrderNode>
        {
            sauteeNode,
            noodlesNode,
            soysauceNode,
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
        if (sauteeNode != null && (sauteeNode.onionTime < 15 || sauteeNode.oilCount < 15 || sauteeNode.bawangCount < 15))
            if (stove_On)
                if (sauteeRoutine == null)
                    sauteeRoutine = StartCoroutine(OnSautee());

                else if (!stove_On && sauteeRoutine != null)
                {
                    // animPot.StopBoil();
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
        }
    }

    private void UpdateNoodleState()
    {
        if (noodlesNode != null && noodlesNode.time < 15)
            if (stove_On)
            {
                if (noodlesRoutine == null)
                    noodlesRoutine = StartCoroutine(OnNoodles());
            }

            else if (!stove_On && noodlesRoutine != null)
            {
                // animPot.StopBoil();
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
                animWok.IngredientChangeState("Noodles", 0);
            if (noodlesNode.time == 15)
                animWok.IngredientChangeState("Noodles", 0);
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
                if (sauteeNode.oilCount == maxCount) return;
                sauteeNode.oilCount += maxCount;
                animWok.PlaceIngredient("Oil");
                break;
            case "Bawang":
                if (sauteeNode.bawangCount == maxCount) return;
                sauteeNode.bawangCount += maxCount;
                animWok.PlaceIngredient("Bawang");
                break;
            case "Onion":
                if (sauteeNode.onionCount == maxCount) return;
                sauteeNode.onionCount += maxCount;
                animWok.PlaceIngredient("Onion");
                break;
            default:
                if (Debug.isDebugBuild) Debug.Log("Unrecognized Type."); break;
        }
        UpdateSauteeState();
    }

    public void AddSoySauce()
    {
        if (soysauceNode == null) soysauceNode = new SoySauceNode();
        if (soysauceNode.count == maxCount) return;

        soysauceNode.count += maxCount;
        StartCoroutine(animWok.AddSoySauce());
    }

    public void AddNoodles()
    {
        if (noodlesNode == null) noodlesNode = new NoodlesNode();

        if (noodlesNode.count == maxCount) return;
        noodlesNode.count += maxCount;

        animWok.PlaceIngredient("Noodles");
        UpdateNoodleState();
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

        if (thickenerNode.count == maxCount) return;
        thickenerNode.count += maxCount;

        animWok.PlaceSlurry("Thickener");
    }

    public void AddEgg()
    {
        if (eggNode == null) eggNode = new EggNode();

        if (eggNode.count == maxCount) return;
        eggNode.count += maxCount;

        animWok.PlaceSlurry("Egg");
    }

    public void OnMix()
    {
        animWok.MixIngredients();

        if ((eggNode != null && eggNode.count > 0 && eggNode.isMixed == false) || (thickenerNode != null && thickenerNode.count > 0 && thickenerNode.isMixed == false))
        {
            if (eggNode.count > 0 && eggNode.isMixed == false)
                eggNode.isMixed = true;

            if (thickenerNode.count > 0 && thickenerNode.isMixed == false)
                thickenerNode.isMixed = true;
                
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
            revertDefaults();
            return;
        }

        if (hitCollider.tag == "Dish")
        {
            if (!hitCollider.TryGetComponent(out PrepDish dish)) { revertDefaults(); return; }
            if (dish.wokGroup != null) { revertDefaults(); return; }

            wokGroup.children = new List<OrderNode>();

            SauteeNode _sauteeNode = new SauteeNode();
            if (sauteeNode != null)
            {
                if (sauteeNode.oilCount >= 1)
                {
                    _sauteeNode.oilCount++;
                    _sauteeNode.oilTime = sauteeNode.oilTime;
                }

                if (sauteeNode.onionCount >= 1)
                {
                    _sauteeNode.onionCount++;
                    _sauteeNode.onionTime = sauteeNode.onionTime;
                }

                if (sauteeNode.bawangCount >= 1)
                {
                    _sauteeNode.bawangCount++;
                    _sauteeNode.bawangTime = sauteeNode.bawangTime;
                }
            }
            wokGroup.children.Add(_sauteeNode);

            NoodlesNode _noodlesNode = new NoodlesNode();
            if (noodlesNode != null && noodlesNode.count >= 1)
            {
                _noodlesNode.count++;
                _noodlesNode.time = noodlesNode.time;
            }
            wokGroup.children.Add(_noodlesNode);

            SoySauceNode _soySauceNode = new SoySauceNode();
            if (soysauceNode != null && soysauceNode.count >= 1)
            {
                _soySauceNode.count++;
            }
            wokGroup.children.Add(_soySauceNode);

            ThickenerNode _thickenerNode = new ThickenerNode();
            if (thickenerNode != null && thickenerNode.count >= 1)
            {
                _thickenerNode.count++;
                _thickenerNode.isMixed = true;
            }
            wokGroup.children.Add(_thickenerNode);

            EggNode _eggNode = new EggNode();
            if (eggNode != null && eggNode.count >= 1)
            {
                _eggNode.count++;
                _eggNode.isMixed = true;
            }
            wokGroup.children.Add(_eggNode);

            //Transfer Components
            dish.potGroup = potGroup;
            dish.wokGroup = wokGroup;

            //Anim Visuals
            animWok.TransferWok(dish);

            //Empty List
            wokGroup = new WokGroup();

            // Clear 1 instance — safely decrement and clamp at 0
            if (sauteeNode != null)
            {
                sauteeNode.oilCount = Mathf.Max(0, sauteeNode.oilCount - 1);
                sauteeNode.onionCount = Mathf.Max(0, sauteeNode.onionCount - 1);
                sauteeNode.bawangCount = Mathf.Max(0, sauteeNode.bawangCount - 1);
            }

            if (noodlesNode != null)
                noodlesNode.count = Mathf.Max(0, noodlesNode.count - 1);

            if (soysauceNode != null)
                soysauceNode.count = Mathf.Max(0, soysauceNode.count - 1);

            if (thickenerNode != null)
                thickenerNode.count = Mathf.Max(0, thickenerNode.count - 1);

            if (eggNode != null)
                eggNode.count = Mathf.Max(0, eggNode.count - 1);


            // Clear All instances if everything is zero
            bool allZero =
                (sauteeNode == null || (sauteeNode.oilCount == 0 && sauteeNode.onionCount == 0 && sauteeNode.bawangCount == 0)) &&
                (noodlesNode == null || noodlesNode.count == 0) &&
                (soysauceNode == null || soysauceNode.count == 0) &&
                (thickenerNode == null || thickenerNode.count == 0) &&
                (eggNode == null || eggNode.count == 0);

            if (allZero)
            {
                sauteeNode = null;
                noodlesNode = null;
                soysauceNode = null;
                thickenerNode = null;
                eggNode = null;
                potGroup = null;
                wokGroup = new WokGroup();

                //Clear 
                animWok.ToggleActive(false);
                if (Debug.isDebugBuild) Debug.Log("Cleared WokNODE");
            }
            else
            {
                animWok.ReduceWokCount();
                if (Debug.isDebugBuild) Debug.Log("Cleared 1 instance of WokNODE");
            }

            revertDefaults();
            return;
        }
        revertDefaults();
        return;
    }
    #endregion
}