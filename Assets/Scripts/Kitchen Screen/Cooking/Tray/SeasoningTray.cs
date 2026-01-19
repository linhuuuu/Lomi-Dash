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

        if (hitCollider.tag == "Trash")
        {
            seasoningSlot.RemoveAllStack();
            revertDefaults();
            return;
        }
        
        revertDefaults();
        return;
    }

}