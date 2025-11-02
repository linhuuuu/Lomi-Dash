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

        if (hitCollider.tag == "Wok")
        {
            if (!hitCollider.TryGetComponent(out CookWok targetWok)) {revertDefaults(); return;}

            if (targetWok.potGroup != null || targetWok.noodlesNode.count > 0) { revertDefaults(); return; }
                
            targetWok.SauteePan(seasoningName);
        }
        revertDefaults();
    }
}