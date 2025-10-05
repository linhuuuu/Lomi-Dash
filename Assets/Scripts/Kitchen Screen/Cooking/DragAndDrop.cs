using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    protected Collider col;
    protected SpriteRenderer sprite;
   
    public int originalSortingOrder { set; get; }
    public Vector3 originalLocalPosition { set; get; }
    public Transform parent { set; get; }
    protected Collider hitCollider;

    private LayerMask interactable;
    private float zOffset;
    protected Camera mainCamera;

    private void Awake()
    {
        mainCamera = CameraManager.cam.mainCam;
        col = gameObject.GetComponent<Collider>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        originalSortingOrder = sprite.sortingOrder;
        originalLocalPosition = transform.localPosition;
        parent = transform.parent;

        interactable += 1 << 8; //interactables are at layer 8
        interactable += 1 << 10; //Trash is at layer 10
        interactable += 1 << 12; //Tray Slots is at layer 10

    }
    private void OnMouseDown()
    {
        zOffset = mainCamera.WorldToScreenPoint(transform.position).z;
        sprite.sortingOrder += 30;
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent(out SpriteRenderer sprite))
                    sprite.sortingOrder += 30;
            }
        }
    }

    private void OnMouseDrag()
    {
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = zOffset;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);

        transform.position = worldPos;
    }
    
    protected Vector3 GetMousePositionInWorldSpace()
    {
        Vector3 p = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        p.z = 0f;
        return p;
    }

    protected void revertDefaults()
    {
        transform.SetParent(parent);
        transform.localPosition = originalLocalPosition;
        sprite.sortingOrder = originalSortingOrder;
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent(out SpriteRenderer sprite))
                    sprite.sortingOrder -= 30;
            }
        }
    }
    protected void initDraggable()
    {
        col.enabled = false;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.blue, 0.2f);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, interactable))
            hitCollider = hit.collider;

        if (hitCollider)
            if (Debug.isDebugBuild) Debug.Log(hitCollider.tag);

        col.enabled = true;
    }
}


