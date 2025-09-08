using UnityEngine;

public class InstBev : DragAndDrop
{
    [SerializeField] private GameObject bevPrefab;
    [SerializeField] private Beverage bevObj;

    void Start()
    {//Edit this
        GetComponent<SpriteRenderer>().sprite = bevObj.sprite;
        transform.parent.GetComponent<SpriteRenderer>().sprite = bevObj.sprite;
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
            newBev.GetComponent<PrepBev>().InitBev(bevObj.id, bevObj.sprite, bevObj.size);

            slot.RecieveBevToSlot(newBev.GetComponent<PrepBev>());

            revertDefaults();
            return;
        }

        revertDefaults();
        return;
    }

}