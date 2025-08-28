using System.Collections;
using System.Collections.Generic;
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
            transform.position = originalPosition;
            return;
        }

        if (hitCollider.TryGetComponent(out CookPot targetPot))
        {
            targetPot.AddBones();
            transform.position = originalPosition;
            return;
        }
    }
}
