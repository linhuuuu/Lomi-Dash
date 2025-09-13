using System;
using UnityEngine;

//Reusable cript to enable gameobject dragging. Dropping is specific to each entity therefore not included here.
public class DragAndDrop : MonoBehaviour
{
    protected Collider2D col;
    protected SpriteRenderer sprite;
    public int originalSortingOrder { set; get; }
    public Vector3 originalLocalPosition { set; get; }
    public Transform parent { set; get; }
    protected Collider2D hitCollider;
    private LayerMask interactable;

    private void Awake()
    {
        col = gameObject.GetComponent<Collider2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        originalSortingOrder = sprite.sortingOrder;
        originalLocalPosition = transform.localPosition;
        parent = transform.parent;

        interactable = 1 << 8; //interactables are at layer 8

    }
    private void OnMouseDown()
    {
        transform.position = GetMousePositionInWorldSpace();
        sprite.sortingOrder += 30;
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent(out SpriteRenderer sprite))
                    sprite.sortingOrder += 30;
            }
        }
    }

    private void OnMouseDrag()
    {
        transform.position = GetMousePositionInWorldSpace();
    }
    protected Vector3 GetMousePositionInWorldSpace()
    {
        Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        p.z = 0f;
        return p;
    }

    protected void revertDefaults()
    {
        transform.SetParent(parent);
        transform.localPosition = originalLocalPosition;
        sprite.sortingOrder = originalSortingOrder;
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent(out SpriteRenderer sprite))
                    sprite.sortingOrder -= 30;
            }
        }
    }
    protected void initDraggable()
    {
        col.enabled = false;
        hitCollider = Physics2D.OverlapPoint(transform.position, interactable);
        col.enabled = true;
        
        if (hitCollider)
            if (Debug.isDebugBuild) Debug.Log(hitCollider.name + " is hit!");
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


