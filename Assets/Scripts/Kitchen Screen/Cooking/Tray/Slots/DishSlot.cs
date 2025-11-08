using PCG;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public class DishSlot : MonoBehaviour
{
    [SerializeField] public PrepTray tray;
    private bool isOccupied;
    private Collider slotCollider;
    private int dishPosSortingOrder;
    [SerializeField] public int dishSlotIndex { private set; get; }
    private PrepDish myDish;

    void Start()
    {
        slotCollider = GetComponent<Collider>();
        dishPosSortingOrder = GetComponent<SpriteRenderer>()?.sortingOrder ?? 0;
    }

    public void RecieveDishToSlot(PrepDish incomingDish)
    {

        if (myDish != null)         //If slot is already contained, remove current dish from that.
            RemoveDishFromSlot(false);

        if (incomingDish.dishSlot != null)
            incomingDish.dishSlot.RemoveDishFromSlot(false);

        //Store Dish and DishSlot References
        myDish = incomingDish;
        myDish.dishSlot = this;
        tray.AddDish(myDish.dishNode, dishSlotIndex);

        // Parent and position
        incomingDish.transform.SetParent(transform);
        incomingDish.transform.localPosition = new Vector3(0f, 0f, 0f);

        // Sorting order
        SortingGroup dishRenderer = incomingDish.GetComponent<SortingGroup>();
        dishRenderer.sortingOrder = dishPosSortingOrder + 1;

        // Store positions
        incomingDish.originalLocalPosition = new Vector3(0f, 0f, 0f);
        myDish.parent = transform;
        incomingDish.originalSortingOrder = dishRenderer.sortingOrder;
        incomingDish.originalSortingGroupOrder = dishRenderer.sortingOrder;

        //toggle collider
        slotCollider.enabled = false;
    }

    public void RemoveDishFromSlot(bool isDishDestroy)
    {
        if (myDish == null) return;

        tray.RemoveDish(myDish.dishNode, dishSlotIndex);
        slotCollider.enabled = true;

        if (isDishDestroy)
        {
            Destroy(myDish.gameObject);
            myDish = null;
            if (Debug.isDebugBuild) Debug.Log("Destoryed Dish");
        }
        else
        {
            myDish = null;
             if (Debug.isDebugBuild) Debug.Log("Removed Dish from Slot");
        }
       
    }

    public void DestroyDish()
    {
        RemoveDishFromSlot(true);
    }

    public void SwapDishesInTray(PrepDish dish1, PrepDish dish2)
    {
        tray.SwapDish(dish1.dishSlot.dishSlotIndex, dish2.dishSlot.dishSlotIndex);
    }
}