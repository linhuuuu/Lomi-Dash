using UnityEngine;

public class GetBones : DragAndDrop
{
    public void OnMouseUp()
    {
        sprite.sortingOrder = originalSortingOrder;

        col.enabled = false;
        Collider2D hitCollider = Physics2D.OverlapPoint(transform.position);
        col.enabled = true;

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
