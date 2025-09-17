using System;
using System.Runtime.CompilerServices;
using UnityEngine;

//Reusable cript to enable gameobject dragging. Dropping is specific to each entity therefore not included here.
public class DragAndDrop : MonoBehaviour
{
    protected Collider col;
    protected SpriteRenderer sprite;
    public int originalSortingOrder { set; get; }
    public Vector3 originalLocalPosition { set; get; }
    public Transform parent { set; get; }
    protected Collider hitCollider;
    private LayerMask interactable;
    private float zOffset;

 protected Camera mainCamera;
    private void Awake()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        col = gameObject.GetComponent<Collider>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        originalSortingOrder = sprite.sortingOrder;
        originalLocalPosition = transform.localPosition;
        parent = transform.parent;

        interactable = 1 << 8; //interactables are at layer 8

    }
    private void OnMouseDown()
    {
        zOffset = mainCamera.WorldToScreenPoint(transform.position).z;
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
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = zOffset;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        transform.position = worldPos;
    }
    protected Vector3 GetMousePositionInWorldSpace()
    {
        Vector3 p = mainCamera.ScreenToWorldPoint(Input.mousePosition);
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


        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.blue, 0.2f);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, interactable))
        {
            hitCollider = hit.collider;
        }

        if (hitCollider)
            if (Debug.isDebugBuild) Debug.Log(hitCollider.tag);
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


