
using UnityEngine;
using UnityEngine.EventSystems;

public class KitchenDrag : MonoBehaviour
{
    [SerializeField] private LayerMask interactable;
    [SerializeField] private float dragSpeed = 60f;
    [SerializeField] private Vector2 kitchenSize;
    [SerializeField] private Vector3 inPos = new Vector3(13f, 0f);
    [SerializeField] private Vector3 outPos = new Vector3(40f, 0f);
    public bool isDragging = false;
    private Vector3 dragStart;
    public bool isKitchenFocus = false;

    //Toggle If Kitchen mode or not
    public void ToggleKitchen()
    {
        isKitchenFocus = !isKitchenFocus;

        if (isKitchenFocus)
            LeanTween.move(gameObject, inPos, 0.5f).setEaseOutBounce();

        else
            LeanTween.move(gameObject, outPos, 0.5f).setEaseOutBounce();
    }

    public void Update()
    {
        if (!isKitchenFocus) return;    //return if not in Kitchen

        //if pointer is clicked;
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsTouchOnInteractable())   //if pointer is touching an interactable object
            {
                isDragging = true;
                dragStart = getMousePos();
            }

        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 currentPos = getMousePos();
            Vector3 offset = currentPos - dragStart;

            Vector3 targetPos = transform.position + offset * dragSpeed * Time.deltaTime;
            dragStart = currentPos;

            transform.position = ClampPositionToCamera(targetPos);
        }

        if (Input.GetMouseButtonUp(0))
            isDragging = false;

    }

    private Vector3 ClampPositionToCamera(Vector3 targetPos)
    {
        Vector2 camBounds = getCameraBounds();
        Vector2 kitchenBounds = kitchenSize / 2;

        float minX = -(kitchenBounds.x - camBounds.x);
        float maxX = kitchenBounds.x - camBounds.x;

        float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);

        return new Vector3(clampedX, 0f, 0f);
    }

    private Vector2 getCameraBounds() => new Vector2(Camera.main.orthographicSize, Camera.main.aspect * Camera.main.orthographicSize);
    //Get mouse Position by Input
    private Vector3 getMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0; 
        return worldPos;
    }

    private bool IsTouchOnInteractable()
    {
        if (EventSystem.current?.IsPointerOverGameObject() ?? false) return true;   //UI Collision

         //Checks if mousepoint interacted with collider in layer interactable
        Vector3 worldPos = getMousePos();
        Collider2D hit = Physics2D.OverlapPoint(worldPos, interactable);    

        if (hit != null)
        {
            Physics2D.SyncTransforms();
            Debug.Log("Hit: " + hit.name + " on layer " + LayerMask.LayerToName(hit.gameObject.layer));
            Debug.DrawRay(worldPos, Vector2.up * 0.5f, Color.red, 2f);
        }
        else
        {
            Debug.Log("No hit");
        }

        return hit != null;
    }

    // //IF outside app
    // private void OnApplicationFocus(bool hasFocus)
    // {
    //     if (!hasFocus)
    //     {
    //         isDragging = false; // ✅ Kill drag state
    //     }
    // }

    // private void OnApplicationPause(bool paused)
    // {
    //     if (paused)
    //     {
    //         isDragging = false; // ✅ Pause or quit
    //     }
    // }

}