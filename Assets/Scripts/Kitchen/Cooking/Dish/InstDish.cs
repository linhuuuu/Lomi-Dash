using UnityEngine;

public class InstrDish : DragAndDrop
{
    [SerializeField] private GameObject dishPrefab;
    [SerializeField] private Transform dishPos;

    public void OnMouseUp()
    {
        initDraggable();

        if (hitCollider == null)
        {
            revertDefaults();
            return;
        }

        if (hitCollider.tag == "DishPos")
        {
            var newDish = Instantiate(dishPrefab, Vector3.down, Quaternion.identity, dishPos);

            newDish.transform.localPosition = Vector3.zero;
            newDish.GetComponent<PrepDish>().originalLocalPosition = Vector3.zero;

            dishPos.GetComponent<Collider2D>().enabled = false;

            revertDefaults();
            return;
        }

        revertDefaults();
        return;
    }

}