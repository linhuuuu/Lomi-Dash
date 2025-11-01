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
            if (targetWok.potGroup == null || targetWok.noodlesNode == null) { revertDefaults(); return; }
            
            targetWok.AddThickener();
        }
        revertDefaults();
    }
}