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

    private LayerMask interactable;
    private float zOffset;
    protected Camera mainCamera;

    // 🔖 Track if we modified sorting
    private bool didAdjustSorting = false;

    System.Collections.IEnumerator getCamera()
    {
        yield return new WaitForSeconds(0.01f);
        mainCamera = CameraManager.cam.mainCam;
    }

    private void Awake()
    {
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

        interactable += 1 << 8; // Interactables
        interactable += 1 << 10; // Trash
        interactable += 1 << 12; // Tray Slots
    }

    protected void OnMouseDown()
    {
        if (UIUtils.IsPointerOverUI()) { revertDefaults(); return; }

        CameraDragZoomControl.isCameraDraggingEnabled = false;
        zOffset = mainCamera.WorldToScreenPoint(transform.position).z;

        if (sortingGroup != null)
        {
            sortingGroup.sortingLayerName = "OnDrag";
        }
        else
        {
            sprite.sortingLayerName = "OnDrag";
            AddToSortingOrder(this.transform, 30);
            didAdjustSorting = true; // ✅ Mark as adjusted
        }
    }

    protected void OnMouseDrag()
    {
        if (UIUtils.IsPointerOverUI()) { revertDefaults(); return; }

        Vector3 screenPos = Input.mousePosition;
        screenPos.z = zOffset;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        transform.position = worldPos;

        AutoNudgeKitchen(screenPos);
    }

    private void AutoNudgeKitchen(Vector3 screenPos)
    {
        if (KitchenDrag.Instance == null) return;

        float edgeThreshold = 150f;

        if (screenPos.x < edgeThreshold)
        {
            KitchenDrag.Instance.NudgeKitchen(1f);
        }
        else if (screenPos.x > Screen.width - edgeThreshold)
        {
            KitchenDrag.Instance.NudgeKitchen(-1f);
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
        if (hitCollider != null && hitCollider.TryGetComponent(out GameObject obj))
            if (Debug.isDebugBuild) Debug.Log("Hit: " + obj);

        transform.SetParent(parent);
        transform.localPosition = originalLocalPosition;

        if (sortingGroup != null)
        {
            sortingGroup.sortingLayerName = originalSortingGroup;
            sortingGroup.sortingOrder = originalSortingGroupOrder;
        }
        else
        {
            sprite.sortingLayerName = originalSortingLayer;

            // ✅ Only decrease if we actually increased it
            if (didAdjustSorting)
            {
                DecreaseSortingOrder(this.transform, 30);
                didAdjustSorting = false; // Reset
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

        if (hitCollider && Debug.isDebugBuild)
            Debug.Log(hitCollider.tag);

        col.enabled = true;
    }

    // ✅ Recursively increase sorting order by offset
    private void AddToSortingOrder(Transform t, int offset)
    {
        if (t.TryGetComponent(out SpriteRenderer sr))
            sr.sortingOrder += offset;

        if (t.TryGetComponent(out SpriteMask mask))
            mask.frontSortingOrder += offset;

        for (int i = 0; i < t.childCount; i++)
        {
            AddToSortingOrder(t.GetChild(i), offset);
        }
    }

    // ✅ Recursively decrease sorting order by offset
    private void DecreaseSortingOrder(Transform t, int offset)
    {
        if (t.TryGetComponent(out SpriteRenderer sr))
            sr.sortingOrder -= offset;

        if (t.TryGetComponent(out SpriteMask mask))
            mask.frontSortingOrder -= offset;

        for (int i = 0; i < t.childCount; i++)
        {
            DecreaseSortingOrder(t.GetChild(i), offset);
        }
    }
}