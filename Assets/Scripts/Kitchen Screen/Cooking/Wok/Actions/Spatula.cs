using PCG;
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

        if (hitCollider.TryGetComponent(out CookWok targetWok))
        {
            if (targetWok.potGroup != null || targetWok.noodlesNode != null )
            targetWok.OnMix();
            revertDefaults();
            return;
        }
        revertDefaults();
    }
}
