using UnityEngine;
using UnityEngine.EventSystems;

public class TrayDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler
{
    [Header("Interaction Settings")]
    [SerializeField] private Camera worldCamera; // Reference to your main 3D camera
    [SerializeField] private float interactionDepth = 771f; // Z of kitchen

    private Vector3 originalPos;
    private RectTransform rectTransform;
    private Canvas canvas;


    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (worldCamera == null)
            worldCamera = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPos = worldCamera.WorldToScreenPoint(transform.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Step 1: Project mouse into 3D world at kitchen depth
        Vector3 worldPos = GetWorldPositionOnPlane(eventData.position);

        // Step 2: Reproject that world point back to screen space
        Vector2 screenPoint = worldCamera.WorldToScreenPoint(worldPos);

        // Step 3: Convert screen point to UI anchored position
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPoint,
            canvas.worldCamera,
            out Vector2 localPoint))
        {
            
            rectTransform.anchoredPosition = localPoint;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
         // Step 3: Convert screen point to UI anchored position
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            originalPos,
            canvas.worldCamera,
            out Vector2 localPoint))
        {
            
            rectTransform.anchoredPosition = localPoint;
        }

        Debug.Log("Dropped at: " + rectTransform.anchoredPosition);
    }

    private Vector3 GetWorldPositionOnPlane(Vector2 screenPos)
    {
        Ray ray = worldCamera.ScreenPointToRay(screenPos);
        Plane plane = new Plane(Vector3.up, new Vector3(0, 0, interactionDepth));
        
        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return ray.GetPoint(1000f);
    }

}