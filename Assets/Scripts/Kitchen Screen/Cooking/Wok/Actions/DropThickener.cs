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
            if (targetWok.potGroup == null || targetWok.noodlesNode.count == 0) { revertDefaults(); return; }
            
            targetWok.AddThickener();
        }
        revertDefaults();
    }
}