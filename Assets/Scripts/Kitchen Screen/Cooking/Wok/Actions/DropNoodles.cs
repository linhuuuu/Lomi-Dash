using UnityEngine;
public class DropNoodles : DragAndDrop
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
            if (targetWok.mix_1_Node == null)
            {
                targetWok.AddNoodles();
                revertDefaults();
                return;
            }
        }
        revertDefaults();
    }
}