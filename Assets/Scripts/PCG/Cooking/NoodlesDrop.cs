using UnityEngine;
public class NoodlesDrop : DragAndDrop
{
    [SerializeField] private string seasoningName;
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
            targetWok.AddEgg();
            revertDefaults();
            return;
        }
        revertDefaults();
    }
}