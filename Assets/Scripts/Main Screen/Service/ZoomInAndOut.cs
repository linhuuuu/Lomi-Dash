using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    [Header("Zoom Settings")]
    public bool enablePinchZoom = true;
    public bool enableDragZoom = true;
    
    [Header("Zoom Limits")]
    public float minOrthographicSize = 1f;
    public float maxOrthographicSize = 20f;
    public float zoomSensitivity = 1f;
    public float zoomSmoothSpeed = 10f;

    private Camera cam;
    private float targetOrthographicSize;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null || !cam.orthographic)
        {
            Debug.LogError("CameraZoomController requires an orthographic camera!");
            enabled = false;
            return;
        }
        targetOrthographicSize = cam.orthographicSize;
    }

    void Update()
    {
        // Handle pinch zoom (multi-touch)
        if (enablePinchZoom && Input.touchCount >= 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Calculate pinch distance
            float currentDistance = Vector2.Distance(touch0.position, touch1.position);
            float previousDistance = Vector2.Distance(
                touch0.position - touch0.deltaPosition,
                touch1.position - touch1.deltaPosition
            );

            // Calculate zoom direction
            float delta = previousDistance - currentDistance;
            Zoom(delta * zoomSensitivity);
        }
        // Handle drag zoom (single touch vertical drag)
        else if (enableDragZoom && Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                // Use vertical drag for zoom
                float delta = touch.deltaPosition.y;
                Zoom(-delta * zoomSensitivity * 0.1f); // Reduced sensitivity for drag
            }
        }

        // Smoothly interpolate to target size
        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetOrthographicSize,
            zoomSmoothSpeed * Time.deltaTime
        );
    }

    void Zoom(float amount)
    {
        targetOrthographicSize -= amount;
        targetOrthographicSize = Mathf.Clamp(
            targetOrthographicSize,
            minOrthographicSize,
            maxOrthographicSize
        );
    }
}