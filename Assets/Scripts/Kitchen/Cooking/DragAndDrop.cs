using UnityEngine;

//Reusable cript to enable gameobject dragging. Dropping is specific to each entity therefore not included here.
public class DragAndDrop : MonoBehaviour
{
    protected Collider2D col;
    protected SpriteRenderer sprite;
    protected int originalSortingOrder;
    protected Vector3 originalLocalPosition;
    protected Transform parent;
    protected Collider2D hitCollider;
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
    protected void initDraggable()
    {
        sprite.sortingOrder = originalSortingOrder;
        col.enabled = false;
        hitCollider = Physics2D.OverlapPoint(transform.position);
        col.enabled = true;
    }


//Touch Input System
    // private void Update()
    // {
    //         if (Input.touchCount > 0)
    //         {
    //             Touch touch = Input.GetTouch(0);
    //             Vector3 touchPosition = touch.position;
    //             touchPosition.z = 0f;

    //             Collider2D hit.

    //             switch (touch.phase)
    //         {
    //             case TouchPhase.Began:
    //                 if (hit.collider != null &&)
    //                 {
    //                     // Simulate OnMouseDown
    //                     originalPosition = transform.position;
    //                     transform.SetAsFirstSibling();

    //                     // Start drag
    //                     Vector3 worldPos = Camera.main.ScreenToWorldPoint(touch.position);
    //                     transform.position = worldPos;
    //                 }
    //                 break;

    //             case TouchPhase.Moved:
    //                 // Simulate OnMouseDrag
    //                 Vector3 movedPos = touch.position;
    //                 movedPos.z = zOffset;
    //                 transform.position = mainCamera.ScreenToWorldPoint(movedPos);
    //                 break;

    //             case TouchPhase.Ended:
    //             case TouchPhase.Canceled:
    //                 // Simulate OnMouseUp
    //                 transform.position = originalPosition;

    //                 break;
    //         }
    //         }
    // }
}


