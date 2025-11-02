using UnityEngine;
public class DropEgg : DragAndDrop
{
    [SerializeField] private string seasoningName;
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
            if (targetWok.potGroup == null || targetWok.noodlesNode.count == 0) { revertDefaults(); return; }

            Debug.Log("Added Egg");
            targetWok.AddEgg();
        }
        revertDefaults();
    }
}