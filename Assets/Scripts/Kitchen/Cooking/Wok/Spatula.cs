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
            if ((targetWok.noodlesNode != null || targetWok.potGroup != null) && targetWok.thickenerNode == null)
            {
                targetWok.AddThickener();
                revertDefaults();
                return;
            }
        }
        revertDefaults();
    }
}
