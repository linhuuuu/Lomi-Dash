using UnityEngine;
public class DropSeasoning : DragAndDrop
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

        if (hitCollider.TryGetComponent(out CookPot targetPot))
        {
            targetPot.AddSeasoning(seasoningName);
            revertDefaults();
            return;
        }
        revertDefaults();
    }
}