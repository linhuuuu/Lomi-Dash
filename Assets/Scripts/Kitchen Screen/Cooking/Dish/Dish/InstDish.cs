using UnityEngine;

public class InstDish : DragAndDrop
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
            newDish.transform.localEulerAngles = Vector3.zero;
            newDish.transform.localScale = Vector3.one;
            newDish.GetComponent<PrepDish>().originalLocalPosition = Vector3.zero;

            dishPos.GetComponent<Collider>().enabled = false;

            revertDefaults();
            return;
        }

        revertDefaults();
        return;
    }

}