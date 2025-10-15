using UnityEngine;

public class InstKnorr : DragAndDrop
{
    public void OnMouseUp()
    {
        initDraggable();

        if (hitCollider == null)
        {
            revertDefaults();
            return;
        }

        if (hitCollider.TryGetComponent(out CookPot targetPot))
        {
            targetPot.AddKnorr();
            revertDefaults();
            return;
        }
    }
}
