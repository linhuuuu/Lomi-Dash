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
            if (hitCollider.transform.childCount > 0) { revertDefaults();  return; }
            var newDish = Instantiate(dishPrefab, Vector3.down, Quaternion.identity, dishPos);

            newDish.transform.localPosition = new Vector3(0f, 0.75f, 0f);
            newDish.transform.localEulerAngles = Vector3.zero;
            newDish.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            newDish.GetComponent<PrepDish>().originalLocalPosition = newDish.transform.localPosition;

            dishPos.GetComponent<Collider>().enabled = false;

            revertDefaults();
            return;
        }

        revertDefaults();
        return;
    }

}