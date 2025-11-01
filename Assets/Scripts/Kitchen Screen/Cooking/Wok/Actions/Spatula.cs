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
        { targetWok.Mix_1();
            revertDefaults();
            return;
            // if ((targetWok.noodlesNode != null || targetWok.potGroup != null) && targetWok.thickenerNode == null)
            // {
            //     if (targetWok.mix_1_Node == null)
            //     {
            //         targetWok.Mix_1();
            //         revertDefaults();
            //         return;
            //     }

            //     targetWok.Mix_2();
            //     revertDefaults();
            //     return;
            // }
        }
        revertDefaults();
    }
}
