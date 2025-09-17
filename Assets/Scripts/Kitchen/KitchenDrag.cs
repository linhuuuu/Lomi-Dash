using UnityEngine;
using UnityEngine.EventSystems;

public class KitchenDrag : MonoBehaviour
{
    [SerializeField] private LayerMask interactable;
    [SerializeField] private Vector2 kitchenSize;
    [SerializeField] private Vector3 inPos;
    [SerializeField] private Vector3 outPos;
    [SerializeField] private Vector3 maxOut;

    private float interactionDepth;

    public bool isDragging = false;
    public bool isKitchenFocus = false;

    private Vector3 dragStartOffset; // object position - mouse world
    private Camera mainCam;

    public float zOffset;

    void Start()
    { mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        interactionDepth = transform.position.z;
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
        if (!isKitchenFocus) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!IsTouchOnInteractable())
            {
                isDragging = true;
                Vector3 mouseWorld = getMousePos();
                dragStartOffset = transform.position - mouseWorld; // full 3D offset
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 mouseWorld = getMousePos();
            Vector3 targetPos = mouseWorld + dragStartOffset;

            transform.position = ClampPosition(targetPos);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    private Vector3 ClampPosition(Vector3 target)
    {
        // Define allowed drag range:
        float minAllowedX = -390f;   // furthest back user can drag
        float maxAllowedX = outPos.x; // never go past outPos.x (-330)

        // Clamp X to this interactive window
        float clampedX = Mathf.Clamp(target.x, minAllowedX, maxAllowedX);

        // Now interpolate Z based on X position
        // From outPos → beyond inPos to -390
        float t = Mathf.InverseLerp(outPos.x, minAllowedX, clampedX);
        t = Mathf.Clamp01(t); // 0 = at outPos, 1 = at -390

        float z = Mathf.Lerp(outPos.z, 801f, t); // Z at -390 is 801

        return new Vector3(clampedX, transform.position.y, z);
    }

    private Vector3 getMousePos()
    {
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = mainCam.WorldToScreenPoint(new Vector3(0, 0, interactionDepth)).z;
        return mainCam.ScreenToWorldPoint(screenPos);
    }

    private RaycastHit hit;
    private bool IsTouchOnInteractable()
    {
        // UI Blocking (still valid)
        if (EventSystem.current.IsPointerOverGameObject() == false)
            return true;

        // Cast a ray from camera through mouse into the world
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

        // Do a 3D raycast
        if (Physics.Raycast(ray, out hit, 1000f, interactable))
        {
            // Check if hit object is part of the kitchen/interactable layer
            Debug.Log("Hit: " + hit.collider.name);
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
            return true;
        }

        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red);
        return false;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) isDragging = false;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        isDragging = false;
    }
}