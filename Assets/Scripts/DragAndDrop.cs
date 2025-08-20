using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour
{
    private Collider2D col;
    private Vector3 startDragPosition;

    private void OnMouseDown()
    {
        // // Only run if no touches (prioritize touch in Update)
        // if (Input.touchCount == 0)
        {
            startDragPosition = transform.position;
            transform.position = GetMouseWorldPos();
        }
    }

    private void OnMouseDrag()
    {

            transform.position = GetMouseWorldPos();

    }

    private void OnMouseUp()
    {

            transform.position = startDragPosition;
    }

    // // Handle mobile touch
    // private void Update()
    // {
    //     if (Input.touchCount == 0) return;

    //     Touch touch = Input.GetTouch(0);
    //     Vector3 touchPos = GetMouseWorldPos();

    //     // Only respond if touch started on this object
    //     if (touch.phase == TouchPhase.Began)
    //     {
    //         if (GetComponent<Collider2D>().OverlapPoint(touchPos))
    //         {
    //             startDragPosition = transform.position;
    //             transform.position = touchPos;
    //         }
    //     }
    //     else if (touch.phase == TouchPhase.Moved)
    //     {
    //         transform.position = touchPos;
    //     }
    //     else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
    //     {
    //         transform.position = startDragPosition;
    //     }
    // }

    // Helper: Get world position from screen (mouse or touch)
    private Vector3 GetMouseWorldPos()
    {
        Vector3 z = Input.mousePosition;
        z.z = 0f;
        return z;
    }
}

