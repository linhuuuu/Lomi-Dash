using UnityEngine;
public class DropThickener : DragAndDrop
{
    public void OnMouseUp()
    {
        initDraggable();

        if (hitCollider == null)
        {
            revertDefaults();
            return;
        }

        if (hitCollider.TryGetComponent(out CookWok targetWok))
        {
            targetWok.AddThickener();
            revertDefaults();
            return;
        }
        revertDefaults();
    }
}