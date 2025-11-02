using PCG;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrepDish : DragAndDrop
{
    //Dish Section 
    public DishSectionNode dishNode { private set; get; }
    public ToppingGroup toppingGroup { private set; get; }  //The Topping Section
    public ToppingNode currentTopping { private set; get; } //Current Toppings
    public List<string> ToppingsUnlocked { private set; get; } = new List<string>() { "Kikiam", "Bola-Bola" };
    public Transform toppingSection { set; get; }
    public AnimDish animDish;

    //Groups
    public PotGroup potGroup { set; get; }
    public WokGroup wokGroup { set; get; }
    public bool isLarge { set; get; }
    public DishSlot dishSlot { set; get; }

    [field: SerializeField] public ToppingDetailCanvas toppingObjDetail { set; get; }
    private Transform dishPos;
    private Collider dishPosCollider;

    public Transform dishTray;

    void Start()
    {
        animDish = GetComponent<AnimDish>();
        //Init DishNode
        if (dishNode == null)
            dishNode = new DishSectionNode();
        dishNode.isLarge = isLarge;
        currentTopping = new ToppingNode("");

        //Set Parent

        dishPos = transform.parent;
        dishPosCollider = dishPos.GetComponent<Collider>();

        toppingSection = transform.Find("Topping Section");
    }
    public void InitDish()
    {
        if (dishNode == null) dishNode = new DishSectionNode();
        if (toppingGroup == null) toppingGroup = new ToppingGroup();
        if (potGroup == null) potGroup = new PotGroup();
        if (wokGroup == null) wokGroup = new WokGroup();
    }
    public void CreateDishNode()
    {
        InitDish();
        AddTopping(currentTopping); //Add leftover toppings;

        if (dishNode.children.Count < 0)
        {
            dishNode.children = new List<OrderNode>
            {
                potGroup,
                wokGroup,
                toppingGroup,
            };
            return;
        }

        //if dishnode children already exists
        dishNode.children.RemoveAll(child => child is PotGroup);
        dishNode.children.Add(potGroup);

        dishNode.children.RemoveAll(child => child is WokGroup);
        dishNode.children.Add(wokGroup);

        dishNode.children.RemoveAll(child => child is ToppingGroup);
        dishNode.children.Add(toppingGroup);
    }

    //Topppings
    public void PlaceTopping(string type)
    {
        if (wokGroup == null || potGroup == null) return;

        if (type == currentTopping.id)
        {
            currentTopping.count++;
        }
        else
        {
            if (currentTopping.count > 0)
            {
                AddTopping(currentTopping);
            }
            currentTopping.id = type;   //if topping does not match prev id, change it to current.
            currentTopping.count = 1;    //reset currentTopping count to 1 if new.
        }
    }

    public void AddTopping(ToppingNode top)
    {
        if (toppingGroup == null) toppingGroup = new ToppingGroup();

        var toppingList = toppingGroup.children;
        if (!toppingList.Any(t => t.id == top.id))
        {
            toppingList.Add(new ToppingNode(top.id));
        }

        foreach (ToppingNode topping in toppingList)
        {
            if (topping.id == top.id)
            {
                topping.count += top.count;
            }
        }
    }

    #region Dropping
    public void OnMouseUp()
    {
        foreach (Collider top in toppingSection.GetComponentsInChildren<Collider>())
            top.enabled = false;

        initDraggable();

        foreach (Collider top in toppingSection.GetComponentsInChildren<Collider>())
            top.enabled = true;

        if (toppingSection.childCount > 0)
        {

        }

        if (hitCollider == null)
        {
            if (Debug.isDebugBuild) Debug.Log("Got Nothing");
            revertDefaults();

            return;
        }

        if (hitCollider.tag == "DishPos")
        {
            OnDishPosActions();
            revertDefaults();
            dishPosCollider.enabled = false;

            return;
        }

        if (hitCollider.tag == "Dish Slot")
        {
            OnDishSlotActions();
            revertDefaults();

            if (dishPosCollider.enabled == false)
                dishPosCollider.enabled = true;

            return;
        }

        if (hitCollider.tag == "Dish")
        {
            if (!OnDishActions())
                if (Debug.isDebugBuild) Debug.Log("Failed Dish Swapping");

            revertDefaults();
            return;
        }

        if (hitCollider.tag == "Trash")
        {
            if (dishSlot != null)
                dishSlot.RemoveDishFromSlot();

            dishPosCollider.enabled = true;

            foreach (ToppingPoolObj obj in toppingSection.GetComponentsInChildren<ToppingPoolObj>())
                obj.section.ReturnTopping(obj);

            Destroy(this.gameObject);
        }

        revertDefaults();
        return;
    }

    #region OnDropActions
    private void OnDishSlotActions()
    {
        if (dishSlot == null)
            CreateDishNode();

        hitCollider.TryGetComponent(out DishSlot slot);
        slot.RecieveDishToSlot(gameObject.GetComponent<PrepDish>());

        foreach (Collider top in toppingSection.GetComponentsInChildren<Collider>())
            top.enabled = false;

        if (Debug.isDebugBuild) Debug.Log("Placed in Tray at slot");
    }

    private void OnDishPosActions()
    {
        if (dishSlot == null) return;

        dishSlot.RemoveDishFromSlot();
        DishToDishPos(this);

        foreach (Collider top in toppingSection.GetComponentsInChildren<Collider>())
            top.enabled = false;

    }

    private bool OnDishActions()
    {
        hitCollider.TryGetComponent(out PrepDish otherDish);

        if (otherDish.dishSlot != null && dishSlot != null)  //if Both Objects are On Tray
        {
            dishSlot.SwapDishesInTray(this, otherDish);
            SwapDishToDish(otherDish);
            return true;
        }

        if (dishSlot != null && otherDish.dishSlot == null) //if otherdish is on pos and The this Dish is On Tray
        {
            dishSlot.RecieveDishToSlot(otherDish);  // Add The OtherDish to This Slot
            DishToDishPos(this);    //Place This Dish to DishPos

            foreach (Collider top in toppingSection.GetComponentsInChildren<Collider>())
                top.enabled = true;

            return true;
        }

        if (dishSlot == null && otherDish.dishSlot != null) //if Hit Dish is on Pos and this is On Tray
        {
            otherDish.dishSlot.RecieveDishToSlot(this);
            DishToDishPos(otherDish);

            foreach (Collider top in toppingSection.GetComponentsInChildren<Collider>())
                top.enabled = false;
            return true;
        }
        return false;
    }
    #endregion
    #region Helpers
    private void DishToDishPos(PrepDish dish)
    {
        dish.parent = dishPos;    //the blanket
        dish.originalLocalPosition = new Vector3(0f, 0f, 0f);
        dish.originalSortingOrder = dishPos.GetComponent<SpriteRenderer>().sortingOrder + 1;
        dish.dishSlot = null;
        dish.revertDefaults();
    }

    //Change Dish Positions and Transforms
    private void SwapDishToDish(PrepDish otherDish)
    {
        // Temp <- Dish 1
        Transform tempParent = parent;
        Vector3 tempPos = originalLocalPosition;
        int tempSort = originalSortingOrder;
        DishSlot tempDishSlot = dishSlot;

        //Dish 1 <- Dish 2
        parent = otherDish.parent;
        originalLocalPosition = otherDish.originalLocalPosition;
        originalSortingOrder = otherDish.originalSortingOrder;
        dishSlot = otherDish.dishSlot;

        //Dish 2 <- Dish 1
        otherDish.parent = tempParent;
        otherDish.originalLocalPosition = tempPos;
        otherDish.originalSortingOrder = tempSort;
        otherDish.dishSlot = tempDishSlot;
        otherDish.revertDefaults();

        if (Debug.isDebugBuild) Debug.Log("Swapped Dishes");
    }
    #endregion
    #endregion
}