using UnityEngine;
public class InstSeasoningTray : DragAndDrop
{
    [SerializeField] private SeasoningSlot seasoningSlot;
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
            Debug.Log("a");
            seasoningSlot.AddToStack();
            revertDefaults();
            return;

        }
        revertDefaults();
        return;

    }

}