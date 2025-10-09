using UnityEngine;
using System;

public class ShakePotSeasoning : DropPotSeasoning
{
    [Header("Positions")]
    private Transform extendedPos;
    private Transform retractedPos;

    private bool isDragging = false;
    private Vector3 dragStartScreenPos;
    
    private bool hasTriggeredSeasoning = false;
    private System.Action<string> OnAddSeasoning;

    [SerializeField] private string seasoningName;

    public void SetTarget(Transform extended, Transform retracted, Action<string> OnAddSeasoning)
    {
        extendedPos = extended;
        retractedPos = retracted;
        this.OnAddSeasoning += OnAddSeasoning;
    }

    void OnMouseDown()
    {
        dragStartScreenPos = Input.mousePosition;
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 currentScreenPos = Input.mousePosition;
        Vector3 delta = currentScreenPos - dragStartScreenPos;

        if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
        {
            if (delta.y < 0f)
            {
                transform.position = extendedPos.position;

                if (!hasTriggeredSeasoning)
                {
                    OnAddSeasoning?.Invoke(seasoningName);
                    hasTriggeredSeasoning = true;
                }
            }
            else
            {
                transform.position = retractedPos.position;
            }
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

}