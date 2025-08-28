using UnityEngine;
public class Spatula : DragAndDrop
{
public void OnMouseUp()
    {
        initDraggable();

        if (hitCollider == null)
        {
            revertDefaults();
            return;
        }
        revertDefaults();
    }
}
