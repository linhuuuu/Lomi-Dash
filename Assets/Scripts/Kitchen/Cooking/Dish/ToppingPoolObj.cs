using UnityEngine;

public class ToppingPoolObj : DragAndDrop
{
    public InstToppings section { set; get; }
    Plane interactionPlane = new Plane(Vector3.up, new Vector3(0, -18f, 0));
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
            transform.position = GetMousePos();
            originalLocalPosition = transform.localPosition;
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

    private Vector3 GetMousePos()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (interactionPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return ray.GetPoint(1000f);
    }
}