using UnityEngine;
public class AddSoySauce : DragAndDrop
{
    private void OnMouseUp()
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
                targetWok.AddSoySauce();
                revertDefaults();
                return;
            }

        }
        revertDefaults();
        return;
    }
}