using UnityEngine;
public class InstSeasoningTray : DragAndDrop
{
    [SerializeField] private SeasoningSlot seasoningSlot;

    void Start()
    {
                promptSprite = new();
            foreach (SpriteRenderer slot in KitchenDrag.Instance.seasoningTraySlots)
            promptSprite.Add(slot);
    }
    
    private void OnMouseUp()
    {
        initDraggable();

        if (hitCollider == null || seasoningSlot.seasoningTrayCount == 5)
        {
            revertDefaults();
            return;
        }

        if (hitCollider.tag == "Seasoning Tray Slot" || hitCollider.tag == "Tray" || hitCollider.tag == "Seasoning Tray Clone")
        {
            seasoningSlot.AddToStack();
            revertDefaults();
            return;

        }
        revertDefaults();
        return;

    }

}