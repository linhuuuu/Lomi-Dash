using UnityEngine;

//Reusable cript to enable gameobject dragging. Dropping is specific to each entity therefore not included here.
public class DragAndDrop : MonoBehaviour
{
<<<<<<< Updated upstream
=======
    protected Collider2D col;
    protected Collider2D hitCollider;
    protected SpriteRenderer sprite;
>>>>>>> Stashed changes
    protected int originalSortingOrder;
    protected SpriteRenderer sprite;
    protected Vector3 originalPosition;
    protected Collider2D col;
    private void Awake()
    {
        col = gameObject.GetComponent<Collider2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        originalSortingOrder = sprite.sortingOrder;
<<<<<<< Updated upstream
        originalPosition = transform.position;
=======
        originalLocalPosition = transform.localPosition;
        parent = transform.parent;

>>>>>>> Stashed changes
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


<<<<<<< Updated upstream
    // private void Update()
    // {

=======
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
>>>>>>> Stashed changes
    //         if (Input.touchCount > 0)
    //         {
    //             Touch touch = Input.GetTouch(0);
    //             Vector3 touchPosition = touch.position;
<<<<<<< Updated upstream
    //             touchPosition.z = zOffset;

    //             Ray ray = mainCamera.ScreenPointToRay(touchPosition);
    //             RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

    //             switch (touch.phase)
    //             {
    //                 case TouchPhase.Began:
    //                     if (hit.collider != null && hit.collider.gameObject == gameObject)
    //                     {
    //                         // Simulate OnMouseDown
    //                         originalPosition = transform.position;
    //                         zOffset = mainCamera.WorldToScreenPoint(transform.position).z;
    //                         transform.SetAsFirstSibling();

    //                         // Start drag
    //                         Vector3 worldPos = mainCamera.ScreenToWorldPoint(touch.position);
    //                         transform.position = worldPos;
    //                     }
    //                     break;

    //                 case TouchPhase.Moved:
    //                     // Simulate OnMouseDrag
    //                     Vector3 movedPos = touch.position;
    //                     movedPos.z = zOffset;
    //                     transform.position = mainCamera.ScreenToWorldPoint(movedPos);
    //                     break;

    //                 case TouchPhase.Ended:
    //                 case TouchPhase.Canceled:
    //                     // Simulate OnMouseUp
    //                     transform.position = originalPosition;

    //                     break;
    //             }
=======
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
>>>>>>> Stashed changes
    //         }
    // }
}


