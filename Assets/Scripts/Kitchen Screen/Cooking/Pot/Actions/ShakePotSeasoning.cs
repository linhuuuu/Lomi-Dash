using UnityEngine;

public class ShakePotSeasoning : DragAndDrop
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeThreshold = 50f;  // How far to move to trigger
    [SerializeField] private float resetThreshold = 30f;  // How far to reset before re-trigger
    [SerializeField] Vector3 delta; 

    private Vector3 dragStartScreenPos;
    private bool hasShaken = false;

    void OnMouseDrag()
    {
        Vector3 currentScreenPos = Input.mousePosition;
        delta = currentScreenPos - dragStartScreenPos;

        if (!hasShaken)
        {
            float travelDistance = delta.magnitude;
            if (travelDistance >= shakeThreshold)
            {
                Debug.Log(" seasoning added!");

                // Prevent re-trigger until reset
                hasShaken = true;
            }
        }
        else
        {
            // Already shaken — require reset
            float returnDistance = delta.magnitude;
            if (returnDistance <= resetThreshold)
            {
                hasShaken = false;
            }
        }
    }

    void OnMouseDown()
    {
        dragStartScreenPos = Input.mousePosition;
        hasShaken = false;
    }
}


// {
//     [Header("Positions")]
//     private Transform extendedPos;
//     private Transform retractedPos;

//     private bool isDragging = false;
//     private Vector3 dragStartScreenPos;
    
//     private bool hasTriggeredSeasoning = false;
//     private System.Action<string> OnAddSeasoning;

//     [SerializeField] private string seasoningName;

//     public void SetTarget(Transform extended, Transform retracted, Action<string> OnAddSeasoning)
//     {
//         extendedPos = extended;
//         retractedPos = retracted;
//         this.OnAddSeasoning += OnAddSeasoning;
//     }

//     void OnMouseDown()
//     {
//         dragStartScreenPos = Input.mousePosition;
//         isDragging = true;
//     }

//     void OnMouseDrag()
//     {
//         if (!isDragging) return;

//         Vector3 currentScreenPos = Input.mousePosition;
//         Vector3 delta = currentScreenPos - dragStartScreenPos;

//         if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
//         {
//             if (delta.y < 0f)
//             {
//                 transform.position = extendedPos.position;

//                 if (!hasTriggeredSeasoning)
//                 {
//                     OnAddSeasoning?.Invoke(seasoningName);
//                     hasTriggeredSeasoning = true;
//                 }
//             }
//             else
//             {
//                 transform.position = retractedPos.position;
//             }
//         }
//     }

//     void OnMouseUp()
//     {
//         isDragging = false;
//     }
