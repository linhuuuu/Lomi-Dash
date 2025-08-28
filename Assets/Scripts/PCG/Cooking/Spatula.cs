using UnityEngine;
public class Spatula : DragAndDrop
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
        revertDefaults();
    }
}
