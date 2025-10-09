using UnityEngine;
using UnityEngine.UI;
public class DropPotSeasoning : DragAndDrop
{
    // [SerializeField] private SpriteRenderer spriteRenderer;
    // [SerializeField] private Button button;
    // [SerializeField] private SpriteRenderer outline;
    
        private void OnMouseUp()
    {
        initDraggable();
        if (hitCollider == null)
        {
            revertDefaults();
            return;
        }

        if (hitCollider.tag == "Pot")
        {
            if (!hitCollider.TryGetComponent(out CookPot targetPot)) return;
            return;
        }

        revertDefaults();
    }

}