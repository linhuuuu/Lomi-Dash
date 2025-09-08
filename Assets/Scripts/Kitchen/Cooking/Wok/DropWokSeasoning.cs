using UnityEngine;
public class DropWokSeasoning : DragAndDrop
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
            targetWok.SauteePan(seasoningName);
            revertDefaults();
            return;
        }
        revertDefaults();
    }
}