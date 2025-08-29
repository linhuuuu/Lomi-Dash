using UnityEngine;

public class DropBones : DragAndDrop
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
            targetPot.AddBones();
            revertDefaults();
            return;
        }
    }
}
