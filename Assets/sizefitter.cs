using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class AutoScaleChildrenWithParent : MonoBehaviour
{
    private RectTransform parentRect;
    private List<ChildData> childrenData = new List<ChildData>();
    
    private class ChildData
    {
        public RectTransform rect;
        public Vector2 normalizedPosition; // relative to parent center
        public Vector2 normalizedSize;
    }

    void Start()
    {
        parentRect = (RectTransform)transform;
        
        // Capture initial layout of all direct children
        foreach (RectTransform child in parentRect)
        {
            Vector2 parentSize = parentRect.rect.size;
            
            // Normalize position (assuming parent pivot is center)
            Vector2 localPos = child.anchoredPosition;
            Vector2 normPos = new Vector2(
                (localPos.x + parentSize.x * 0.5f) / parentSize.x,
                (localPos.y + parentSize.y * 0.5f) / parentSize.y
            );

            // Normalize size
            Vector2 normSize = new Vector2(
                child.sizeDelta.x / parentSize.x,
                child.sizeDelta.y / parentSize.y
            );

            childrenData.Add(new ChildData
            {
                rect = child,
                normalizedPosition = normPos,
                normalizedSize = normSize
            });
        }
    }

#if UNITY_EDITOR
    void LateUpdate()
    {
        Vector2 currentParentSize = parentRect.rect.size;

        foreach (var data in childrenData)
        {
            // Recalculate position (parent pivot = center)
            Vector2 newPos = new Vector2(
                data.normalizedPosition.x * currentParentSize.x - currentParentSize.x * 0.5f,
                data.normalizedPosition.y * currentParentSize.y - currentParentSize.y * 0.5f
            );
            data.rect.anchoredPosition = newPos;

            // Recalculate size
            data.rect.sizeDelta = new Vector2(
                data.normalizedSize.x * currentParentSize.x,
                data.normalizedSize.y * currentParentSize.y
            );
        }
    }
    #endif
}