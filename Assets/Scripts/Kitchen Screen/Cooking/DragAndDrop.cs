using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class DragAndDrop : MonoBehaviour
{
    protected Collider col;
    protected SpriteRenderer sprite;
    public SortingGroup sortingGroup { set; get; }

    public int originalSortingOrder { set; get; }
    public int originalSortingGroupOrder { set; get; }
    public string originalSortingGroup { set; get; }
    public string originalSortingLayer { set; get; }
    public Vector3 originalLocalPosition { set; get; }
    public Transform parent { set; get; }
    protected Collider hitCollider;

    private bool isDragged = false;

    private LayerMask interactable;
    private float zOffset;
    protected Camera mainCamera;

    System.Collections.IEnumerator getCamera()
    {
        yield return new WaitForSeconds(0.01f);
        mainCamera = CameraManager.cam.mainCam;
    }

    private void Awake()
    {
        //mainCamera = CameraManager.cam.mainCam;
        StartCoroutine(getCamera());
        col = gameObject.GetComponent<Collider>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        originalSortingOrder = sprite.sortingOrder;
        originalLocalPosition = transform.localPosition;

        if (transform.TryGetComponent(out SortingGroup group))
        {
            sortingGroup = group;
            originalSortingGroup = group.sortingLayerName;
            originalSortingGroupOrder = group.sortingOrder;
        }

        originalSortingLayer = sprite.sortingLayerName;

        parent = transform.parent;

        interactable += 1 << 8; //interactables are at layer 8
        interactable += 1 << 10; //Trash is at layer 10
        interactable += 1 << 12; //Tray Slots is at layer 10
    }


    private void OnMouseDown()
    {
        if (UIUtils.IsPointerOverUI()) { revertDefaults(); return; }

        CameraDragZoomControl.isCameraDraggingEnabled = false;

        zOffset = mainCamera.WorldToScreenPoint(transform.position).z;

        if (sortingGroup != null)
            sortingGroup.sortingLayerName = "OnDrag";

        if (sortingGroup == null)
            sprite.sortingLayerName = "OnDrag";

        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent(out SpriteRenderer sprite))
                    sprite.sortingOrder += 30;

                if (transform.GetChild(i).TryGetComponent(out SpriteMask mask))
                    mask.frontSortingOrder += 30;
            }
        }
    }


    private void OnMouseDrag()
    {
        if (UIUtils.IsPointerOverUI() && !isDragged) { revertDefaults(); return; }

        Vector3 screenPos = Input.mousePosition;
        screenPos.z = zOffset;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);

        transform.position = worldPos;

        AutoNudgeKitchen(screenPos);
        isDragged = true;
    }

    private void AutoNudgeKitchen(Vector3 screenPos)
    {
        if (KitchenDrag.Instance == null) return;

        float edgeThreshold = 100f;

        if (screenPos.x < edgeThreshold)
        {
            KitchenDrag.Instance.NudgeKitchen(1f); // move kitchen left  
        }
        else if (screenPos.x > Screen.width - edgeThreshold)
        {
            KitchenDrag.Instance.NudgeKitchen(-1f); // move kitchen right
        }
    }

    protected Vector3 GetMousePositionInWorldSpace()
    {
        Vector3 p = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        p.z = 0f;
        return p;
    }

    protected void revertDefaults()
    {
        CameraDragZoomControl.isCameraDraggingEnabled = false;
        transform.SetParent(parent);
        transform.localPosition = originalLocalPosition;
        isDragged = false;

        if (sortingGroup != null)
        {
            sortingGroup.sortingLayerName = originalSortingGroup;
            sortingGroup.sortingOrder = originalSortingGroupOrder;
        }

        if (sortingGroup == null)
            sprite.sortingLayerName = originalSortingLayer;

        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent(out SpriteRenderer sprite))
                    sprite.sortingOrder -= 30;

                if (transform.GetChild(i).TryGetComponent(out SpriteMask mask))
                    mask.frontSortingOrder -= 30;

                if (transform.GetChild(i).TryGetComponent(out Collider childCol))
                    childCol.enabled = true;
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


