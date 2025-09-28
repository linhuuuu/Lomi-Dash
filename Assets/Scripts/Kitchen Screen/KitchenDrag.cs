using UnityEngine;
using UnityEngine.EventSystems;

public class KitchenDrag : MonoBehaviour
{
    [SerializeField] private LayerMask interactable;
    [SerializeField] private Vector3 inPos;
    [SerializeField] private Vector3 outPos;

    public bool isDragging = false;
    public bool isKitchenFocus = false;

    private Vector3 dragStartOffset;
    private Camera mainCam;
    private int activeTouchId = -1; // Track which touch we're using

    void Start()
    {
        mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    public void ToggleKitchen()
    {
        isKitchenFocus = !isKitchenFocus;

        if (isKitchenFocus)
            LeanTween.move(gameObject, inPos, 0.5f).setEaseOutBounce();
        else
            LeanTween.move(gameObject, outPos, 0.5f).setEaseOutBounce();
    }

    void Update()
    {
        // Only run if kitchen is focused
        if (!isKitchenFocus) return;

        Debug.Log("a");
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        // Skip if touching UI
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // Process all touches
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            // Convert touch position to world point
            Vector3 touchWorld = GetWorldPosition(touch.position);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Only start dragging if not blocked by raycast
                    if (activeTouchId == -1 && !IsTouchOnInteractableLayer(touchWorld))
                    {
                        activeTouchId = touch.fingerId;
                        isDragging = true;
                        dragStartOffset = transform.position - touchWorld;
                    }
                    break;

                case TouchPhase.Moved:
                    if (touch.fingerId == activeTouchId)
                    {
                        Vector3 targetPos = touchWorld + dragStartOffset;
                        transform.position = ClampPosition(targetPos);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (touch.fingerId == activeTouchId)
                    {
                        EndDrag();
                    }
                    break;
            }
        }

        // Also handle mouse as fallback (for editor testing)
        if (Application.isEditor)
        {
            HandleMouseInput();
        }
    }

    private void HandleMouseInput()
    {
        Vector3 mouseWorld = getMousePos();

        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject() && !IsTouchOnInteractableLayer(mouseWorld))
            {
                isDragging = true;
                dragStartOffset = transform.position - mouseWorld;
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 targetPos = mouseWorld + dragStartOffset;
            transform.position = ClampPosition(targetPos);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    private Vector3 GetWorldPosition(Vector2 screenPos)
    {
        // Adjust z based on camera and object depth
        Vector3 screenPoint = new Vector3(screenPos.x, screenPos.y, mainCam.WorldToScreenPoint(transform.position).z);
        return mainCam.ScreenToWorldPoint(screenPoint);
    }

    private Vector3 getMousePos()
    {
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = mainCam.WorldToScreenPoint(transform.position).z;
        return mainCam.ScreenToWorldPoint(screenPos);
    }

    private bool IsTouchOnInteractableLayer(Vector3 worldPos)
    {
        Ray ray = mainCam.ScreenPointToRay(worldPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, interactable))
        {
            Debug.Log("Hit: " + hit.collider.name);
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 0.1f);
            return true;
        }
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 0.1f);
        return false;
    }

    private Vector3 ClampPosition(Vector3 target)
    {
        float minAllowedX = -390f;
        float maxAllowedX = outPos.x;

        float clampedX = Mathf.Clamp(target.x, minAllowedX, maxAllowedX);

        float t = Mathf.InverseLerp(outPos.x, minAllowedX, clampedX);
        t = Mathf.Clamp01(t);

        float z = Mathf.Lerp(outPos.z, 801f, t); // Z increases as X goes back

        return new Vector3(clampedX, transform.position.y, z);
    }

    private void EndDrag()
    {
        isDragging = false;
        activeTouchId = -1;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) EndDrag();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        EndDrag();
    }
}