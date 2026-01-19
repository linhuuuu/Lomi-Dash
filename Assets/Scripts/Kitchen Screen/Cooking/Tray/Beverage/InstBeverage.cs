using UnityEngine;

public class InstBev : DragAndDrop
{
    [SerializeField] private GameObject bevPrefab;
    [SerializeField] private Beverage bevObj;
    [SerializeField] private Vector3 offset;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = bevObj.sprite;
        transform.parent.GetComponent<SpriteRenderer>().sprite = bevObj.sprite;

        promptSprite = new();
                foreach (SpriteRenderer slot in KitchenDrag.Instance.beverageSlots)
            promptSprite.Add(slot);
    }

    public Vector3 screenOffset;
    public new void OnMouseDown()
    {
        base.OnMouseDown();
        transform.localPosition += offset;
    }

    public new void OnMouseDrag()
    {
        base.OnMouseDrag();
        transform.localPosition += offset;

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

            //Inst New Bev
            var newBev = Instantiate(bevPrefab, Vector3.zero, Quaternion.identity, slot.transform);
            newBev.GetComponent<PrepBev>().InitBev(bevObj.id, bevObj.sprite, bevObj);

            slot.RecieveBevToSlot(newBev.GetComponent<PrepBev>());

            revertDefaults();
            return;
        }
        revertDefaults();
        return;
    }
}