using UnityEngine;
public class BevSlot : MonoBehaviour
{
    [SerializeField] public PrepTray tray;
    private Collider slotCollider;
    private int bevPosSortingOrder;
    [SerializeField] public int bevSlotIndex { private set; get; }
    private PrepBev myBev;

    void Start()
    {
        slotCollider = GetComponent<Collider>();
        bevPosSortingOrder = GetComponent<SpriteRenderer>()?.sortingOrder ?? 0;
    }

    public void RecieveBevToSlot(PrepBev incomingBev)
    {

        if (incomingBev.bevSlot != null)    //If moving from one slot to another
        {
            incomingBev.bevSlot.RemoveBevFromSlot(false);
        }

        myBev = incomingBev;
        tray.AddBeverage(myBev.bevNode, bevSlotIndex);

        // Parent and position
        myBev.transform.SetParent(transform);
        myBev.transform.localPosition = new Vector3(0f, 1.5f, 0f);
        myBev.transform.localEulerAngles = Vector3.zero;
        myBev.bevSlot = this;

        // Sorting order
        SpriteRenderer dishRenderer = myBev.GetComponent<SpriteRenderer>();
        dishRenderer.sortingOrder = bevPosSortingOrder + 1;

        // Store positions
        myBev.originalLocalPosition = myBev.transform.localPosition;
        myBev.parent = transform;
        myBev.originalSortingOrder = dishRenderer.sortingOrder;
        myBev.GetComponent<BoxCollider>().center = new Vector3(0f, 0.4f, 0f);     //idk why???

        //Toggle collider
        slotCollider.enabled = false;
    }

    public void RemoveBevFromSlot(bool isBevDestroy)
    {
        if (myBev == null) return;

        tray.RemoveBev(myBev.bevNode, bevSlotIndex);
        slotCollider.enabled = true;
        if (isBevDestroy)
        {
            Destroy(myBev.gameObject);
            myBev = null;
        }
        else
        {
            myBev = null;
        }
    }

    public void DestroyBev()
    {
        RemoveBevFromSlot(true);

    }

    public void SwapDishesInTray(PrepDish dish1, PrepDish dish2)
    {
        tray.SwapDish(dish1.dishSlot.dishSlotIndex, dish2.dishSlot.dishSlotIndex);
    }

    public void SwapBevInTray(PrepBev bev1, PrepBev bev2)
    {
        tray.SwapBev(bev1.bevSlot.bevSlotIndex, bev2.bevSlot.bevSlotIndex);
    }
}