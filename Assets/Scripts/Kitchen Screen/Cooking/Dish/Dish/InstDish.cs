using UnityEngine;

public class InstDish : DragAndDrop
{
    [SerializeField] private GameObject dishPrefab;
    [SerializeField] private bool isDishLarge;
    [SerializeField] private AudioSource SRC;
    [SerializeField] private AudioClip placeDish;

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
            var newDish = Instantiate(dishPrefab, Vector3.down, Quaternion.identity, hitCollider.transform);

            newDish.transform.localPosition = new Vector3(0f, 0.75f, 0f);
            newDish.transform.localEulerAngles = Vector3.zero;

            if (!isDishLarge)
                newDish.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
            else
                newDish.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            newDish.GetComponent<PrepDish>().originalLocalPosition = newDish.transform.localPosition;
            newDish.GetComponent<PrepDish>().isLarge = this.isDishLarge;
            
            
            hitCollider.GetComponent<Collider>().enabled = false;

            SRC.PlayOneShot(placeDish, 1f);
            revertDefaults();
            return;
        }

        revertDefaults();
        return;
    }

}