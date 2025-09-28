using UnityEngine;
public class SeasoningTray : DragAndDrop
{
    public SeasoningSlot seasoningSlot {set; get;}
    private void OnMouseUp()
    {
        initDraggable();

        if (hitCollider == null)
        {
            revertDefaults();
            return;
        }

        if (hitCollider.tag == "Seasoning Tray" || hitCollider.tag == "Trash")
        {
            seasoningSlot.RemoveStack();
            revertDefaults();
            return;
        }
        
        revertDefaults();
        return;
    }

}