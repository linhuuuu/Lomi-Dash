using PCG;
using UnityEngine;

public class PrepBev : DragAndDrop
{
    public BeverageSectionNode bevNode { set; get; }
    public BevSlot bevSlot;

    public void InitBev(string id, Sprite sprite, Beverage beverage)
    {
        bevNode = new BeverageSectionNode(beverage);
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
    public void OnMouseUp()
    {
        initDraggable();

        if (hitCollider == null)
        {
            revertDefaults();
            return;
        }

        if (hitCollider.tag == "Beverage Slot")
        {
            hitCollider.TryGetComponent(out BevSlot slot);
            slot.RecieveBevToSlot(this);

            revertDefaults();
            return;
        }

        if (hitCollider.tag == "Beverage")
        {
            hitCollider.TryGetComponent(out PrepBev otherBev);

            bevSlot.SwapBevInTray(this, otherBev);
            SwapBevToBev(otherBev);
        }

        if (hitCollider.tag == "Trash")
        {
            bevSlot.RemoveBevFromSlot();
            Destroy(gameObject);
        }

        revertDefaults();
        return;
    }
    
     //Change Bev Positions and Transforms
    private void SwapBevToBev(PrepBev otherBev)
    {
        // Temp <- Bev 1
        Transform tempParent = parent;
        Vector3 tempPos = originalLocalPosition;
        int tempSort = originalSortingOrder;
        BevSlot tempBevSlot = bevSlot;

        //Bev 1 <- Bev 2
        parent = otherBev.parent;
        originalLocalPosition = otherBev.originalLocalPosition;
        originalSortingOrder = otherBev.originalSortingOrder;
        bevSlot = otherBev.bevSlot;

        //Bev 2 <- Bev 1
        otherBev.parent = tempParent;
        otherBev.originalLocalPosition = tempPos;
        otherBev.originalSortingOrder = tempSort;
        otherBev.bevSlot = tempBevSlot;
        otherBev.revertDefaults();

        if (Debug.isDebugBuild) Debug.Log("Swapped Bev");
    }

}