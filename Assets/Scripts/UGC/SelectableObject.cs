using UnityEngine;
using UnityEngine.EventSystems; // 👈 Required for UI raycast

public class UGCObjectController : MonoBehaviour
{
    public static UGCObjectController SelectedObject { get; private set; }

    private Camera cam;
    private Vector3 dragOffset;
    private float initialPinchDistance;
    private float initialScale;
    private bool isDragging = false;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);

            // 👇 NEW: Ignore if touching UI
            if (IsPointerOverUI(t))
                return;

            if (t.phase == TouchPhase.Began)
            {
                if (IsPointerOverObject(t.position))
                {
                    SelectThis();
                    isDragging = true;
                    Vector3 worldTouch = cam.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, 0));
                    dragOffset = transform.position - worldTouch;
                }
                else
                {
                    DeselectAll();
                }
            }
            else if (t.phase == TouchPhase.Moved && isDragging && SelectedObject == this)
            {
                if (IsPointerOverUI(t))
                    return;
                
                Vector3 worldTouch = cam.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, 0));
                Vector3 pos = worldTouch + dragOffset;
                transform.position = new Vector3(pos.x, pos.y, transform.position.z);
            }
            else if (t.phase == TouchPhase.Ended)
            {
                isDragging = false;
            } 
        }
        else if (Input.touchCount == 2 && SelectedObject == this)
        {
            // Optional: also skip if UI is touched? Usually not needed for pinch.
            // But if you want, you could check both fingers — for now, we skip.
            isDragging = false;
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
            {
                initialPinchDistance = Vector2.Distance(t0.position, t1.position);
                initialScale = transform.localScale.x;
            }
            else if (t0.phase == TouchPhase.Moved || t1.phase == TouchPhase.Moved)
            {
                float currentDist = Vector2.Distance(t0.position, t1.position);
                float delta = currentDist / initialPinchDistance;
                float newScale = Mathf.Clamp(initialScale * delta, 0.1f, 3f);
                transform.localScale = Vector3.one * newScale;
            }
        }
    }
bool IsPointerOverUI(Touch touch)
{
    if (EventSystem.current == null)
        return false;

    PointerEventData eventData = new PointerEventData(EventSystem.current)
    {
        position = touch.position,
        pointerId = touch.fingerId
    };

    var results = new System.Collections.Generic.List<RaycastResult>();
    EventSystem.current.RaycastAll(eventData, results);

    // 🔍 DEBUG: Print what UI is being hit
    foreach (var result in results)
    {
        Debug.Log("UI Hit: " + result.gameObject.name + " | RaycastTarget: " +
                  result.gameObject.GetComponent<UnityEngine.UI.Graphic>()?.raycastTarget, result.gameObject);
    }

    return results.Count > 0;
}

    bool IsPointerOverObject(Vector2 screenPos)
    {
        Vector3 worldPoint = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
        Vector2 worldPoint2D = new Vector2(worldPoint.x, worldPoint.y);

        Debug.DrawLine(worldPoint - Vector3.right * 0.1f, worldPoint + Vector3.right * 0.1f, Color.cyan, 1f);
        Debug.DrawLine(worldPoint - Vector3.up * 0.1f, worldPoint + Vector3.up * 0.1f, Color.cyan, 1f);

        Collider2D hit = Physics2D.OverlapPoint(worldPoint2D);
        return hit != null && hit.gameObject == gameObject;
    }

    void SelectThis()
    {
        if (SelectedObject != null && SelectedObject != this)
        {
            SelectedObject.OnDeselected();
        }
        SelectedObject = this;
        OnSelected();
    }

    public static void DeselectAll()
    {
        if (SelectedObject != null)
        {
            SelectedObject.OnDeselected();
            SelectedObject = null;
        }
    }

    void OnSelected()
    {
        Debug.Log("Selected: " + name);
    }

    void OnDeselected()
    {
        Debug.Log("Deselected: " + name);
    }
}