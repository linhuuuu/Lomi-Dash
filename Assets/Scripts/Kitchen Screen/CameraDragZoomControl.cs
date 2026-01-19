using UnityEngine;

public class CameraDragZoomControl : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float minOrthoSize = 1f;
    public float maxOrthoSize = 20f;
    public float zoomSpeed = 2f; // Higher = faster zoom

    [Header("Pan Settings")]
    public float panSpeed = 1f;
    public bool invertPan = false;

    private Camera cam;
    private Vector3 lastTouchPosition;

    public static CameraDragZoomControl instance;
    public static bool isCameraDraggingEnabled = true;

    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {

        cam = GetComponent<Camera>();
        if (cam == null || cam.orthographic == false)
        {
            Debug.LogError("CameraController requires an Orthographic Camera!");
            enabled = false;
        }
    }

    void Update()
    {
        // if (!isCameraDraggingEnabled || KitchenDrag.Instance.isKitchenFocus)
        //     return;
        // HandleZoom();
        // HandlePan();
    }

    void HandleZoom()
    {
        float scroll = 0f;

        // Mouse Scroll
        if (Input.mouseScrollDelta != Vector2.zero)
        {
            scroll = Input.mouseScrollDelta.y;
        }
        // Touch Pinch
        else if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevDistance = Vector2.Distance(touch0PrevPos, touch1PrevPos);
            float currentDistance = Vector2.Distance(touch0.position, touch1.position);

            scroll = (currentDistance - prevDistance) * 0.01f; // Scale down pinch sensitivity
        }

        if (scroll != 0f)
        {
            float newSize = cam.orthographicSize - scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(newSize, minOrthoSize, maxOrthoSize);
        }
    }

    void HandlePan()
    {
        Vector3 panDelta = Vector3.zero;

        // Use RIGHT CLICK to avoid UI conflicts
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            Vector3 mouseDelta = new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0f);
            panDelta = mouseDelta * panSpeed * cam.orthographicSize * 0.1f;

            // Optional: Debug
            // Debug.Log($"Panning: {panDelta}");
        }
        else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDelta = Input.GetTouch(0).deltaPosition;
            panDelta = new Vector3(-touchDelta.x, -touchDelta.y, 0f) * panSpeed * cam.orthographicSize * 0.001f;
        }

        if (panDelta != Vector3.zero)
        {
            if (invertPan) panDelta *= -1f;
            transform.Translate(panDelta, Space.World);
        }
    }

    //Centering
    private Vector3 velocity = Vector3.zero;

    public void CenterOnTarget(Transform target)
    {
        if (target == null) return;

        Vector3 targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);
        LeanTween.move(this.gameObject, targetPos, 0.3f).setEaseInOutCubic();
        LeanTween.value(gameObject, (float newSize) => { cam.orthographicSize = newSize; }, cam.orthographicSize, 5f, 0.4f).setEaseInBounce();
    }
}