using UnityEngine;
public class InstSeasoningTray : DragAndDrop
{
    [SerializeField] private SeasoningSlot seasoningSlot;
    [SerializeField] private GameObject seaTrayObj;
    private void OnMouseUp()
    {
        initDraggable();

        if (hitCollider == null || seasoningSlot.transform.childCount == 5)
        {
            revertDefaults();
            return;
        }

        if (hitCollider.tag == "Seasoning Tray Slot" || hitCollider.tag == "Seasoning Tray Clone")
        {
            seasoningSlot.AddToStack(seaTrayObj);
            revertDefaults();
            return;

        }
        revertDefaults();
        return;

    }

}