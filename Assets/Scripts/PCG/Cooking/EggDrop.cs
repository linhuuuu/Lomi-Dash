using UnityEngine;
public class EggDrop : DragAndDrop
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