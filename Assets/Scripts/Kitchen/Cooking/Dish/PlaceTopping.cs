using UnityEngine;

public class PlaceTopping : DragAndDrop
{
    [SerializeField] private string toppingName;
    private void OnMouseUp()
    {
        initDraggable();

        if (hitCollider == null)
        {
            revertDefaults();
            return;
        }

        if (hitCollider.tag == "Dish")
        {
            hitCollider.TryGetComponent(out PrepDish targetDish);
            targetDish.PlaceTopping(toppingName);

            revertDefaults();
            return;
        }
        revertDefaults();
    }
}