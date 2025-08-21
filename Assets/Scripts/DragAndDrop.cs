using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour
{
    private Vector3 originalPosition;
    private Camera mainCamera;
    private float zOffset;
    private SpriteRenderer spriteRenderer;

    [HideInInspector] public bool Snapped = false;

    [Header("Sprites")]
    public Sprite defaultSprite;
    public Sprite draggingSprite;
    public Sprite sittingSprite;

    private void Awake()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = defaultSprite;
    }

    private void OnMouseDown()
    {
        originalPosition = transform.position;
        zOffset = mainCamera.WorldToScreenPoint(transform.position).z;

        transform.SetAsFirstSibling();

        if (draggingSprite != null)
            spriteRenderer.sprite = draggingSprite;
    }

    private void OnMouseDrag()
    {
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = zOffset;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);

        transform.position = worldPos;
    }

    private void OnMouseUp()
    {
        transform.position = originalPosition;
        if (defaultSprite != null)
            spriteRenderer.sprite = defaultSprite;
    }

    private void Update()
{
#if UNITY_IOS || UNITY_ANDROID
    if (Input.touchCount > 0)
    {
        Touch touch = Input.GetTouch(0);
        Vector3 touchPosition = touch.position;
        touchPosition.z = zOffset;

        Ray ray = mainCamera.ScreenPointToRay(touchPosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    // Simulate OnMouseDown
                    originalPosition = transform.position;
                    zOffset = mainCamera.WorldToScreenPoint(transform.position).z;
                    transform.SetAsFirstSibling();
                    if (draggingSprite != null)
                        spriteRenderer.sprite = draggingSprite;

                    // Start drag
                    Vector3 worldPos = mainCamera.ScreenToWorldPoint(touch.position);
                    transform.position = worldPos;
                }
                break;

            case TouchPhase.Moved:
                // Simulate OnMouseDrag
                Vector3 movedPos = touch.position;
                movedPos.z = zOffset;
                transform.position = mainCamera.ScreenToWorldPoint(movedPos);
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                // Simulate OnMouseUp
                transform.position = originalPosition;
                if (defaultSprite != null)
                    spriteRenderer.sprite = defaultSprite;
                break;
        }
    }
#endif
}
}


