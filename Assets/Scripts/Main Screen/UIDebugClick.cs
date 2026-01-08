using UnityEngine;
using UnityEngine.EventSystems;

public class UIDebugClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            Debug.Log($"[UI Click] Hit: {eventData.pointerCurrentRaycast.gameObject.name} (Layer: {LayerMask.LayerToName(eventData.pointerCurrentRaycast.gameObject.layer)})", 
                      eventData.pointerCurrentRaycast.gameObject);
        }
        else
        {
            Debug.Log("[UI Click] No UI element hit (clicked on canvas background or empty space).");
        }
    }
}