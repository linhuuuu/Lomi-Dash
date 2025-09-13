using UnityEngine;

public class ToppingPoolObj : DragAndDrop
{
    public InstToppings section { set; get;}
    public void OnMouseUp()
    {
        initDraggable();

        if (hitCollider == null)
        {
            revertDefaults();
            return;
        }

        if (hitCollider.tag == "Dish")
        {
            originalLocalPosition = GetMousePositionInWorldSpace();
            revertDefaults();
            return;
        }

        if (hitCollider.tag == "Trash")
        {
            section.ReturnTopping();
            revertDefaults();
            return;
        }

        revertDefaults();
        return;
    }
}