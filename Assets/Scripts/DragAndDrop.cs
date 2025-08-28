using UnityEngine;

//Reusable cript to enable gameobject dragging. Dropping is specific to each entity therefore not included here.
public class DragAndDrop : MonoBehaviour
{
    protected Collider2D col;
    protected SpriteRenderer sprite;
    protected int originalSortingOrder;
    protected Vector3 originalLocalPosition;
    protected Transform parent;
    private void Awake()
    {
        col = gameObject.GetComponent<Collider2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        originalSortingOrder = sprite.sortingOrder;
        originalLocalPosition = transform.localPosition;
        parent = transform.parent;
    }
    private void OnMouseDown()
    {
        transform.position = GetMousePositionInWorldSpace();
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 20;
    }

    private void OnMouseDrag()
    {
        transform.position = GetMousePositionInWorldSpace();
    }
    private Vector3 GetMousePositionInWorldSpace()
    {
        Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        p.z = 0f;
        return p;
    }

    protected void revertDefaults()
    {
        transform.localPosition = originalLocalPosition;
    }

}


