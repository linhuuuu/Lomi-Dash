using System.Collections.Generic;
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

    protected List<SpriteRenderer> promptSprite { set; get; }

    private LayerMask interactable;
    private float zOffset;
    protected Camera mainCamera;

    // 🔖 Track if we modified sorting
    private bool didAdjustSorting = false;

    private void Awake()
    {
        mainCamera = CameraManager.cam.mainCam;
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

        interactable = (1 << 8) | (1 << 10) | (1 << 12);
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
            didAdjustSorting = true;
        }
    }

    protected void OnMouseDrag()
    {
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = zOffset;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        transform.position = worldPos;

        //outline each prompt
        if (promptSprite != null)
        {
            foreach (SpriteRenderer prompt in promptSprite)
            {
                prompt.gameObject.SetActive(true);
            }
        }

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

            //remove outline each prompt
        if (promptSprite != null)
        {
            foreach (SpriteRenderer prompt in promptSprite)
            {
                prompt.gameObject.SetActive(false);
            }
        }
    }

    protected void initDraggable()
    {
        KitchenDrag.Instance.SpecifyTouched(name);
        
        if (mainCamera == null)
        {
            Debug.LogWarning("mainCamera is null in initDraggable!");
            return;
        }

        col.enabled = false;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        bool hitSuccess = Physics.Raycast(ray, out RaycastHit hit, 100f, interactable);

        Color rayColor = hitSuccess ? Color.green : Color.red;
        Debug.DrawRay(ray.origin, ray.direction * 100f, rayColor, 2f); // 2s visibility

        if (hitSuccess)
        {
            hitCollider = hit.collider;
            if (Debug.isDebugBuild)
            {
                Debug.Log($"[Drag] Hit: {hitCollider.name} (Layer: {LayerMask.LayerToName(hitCollider.gameObject.layer)})");
            }
        }
        else
        {
            hitCollider = null;
            if (Debug.isDebugBuild)
            {
                Debug.Log("[Drag] No valid collider hit under mouse (check LayerMask or UI blocking).");
            }
        }

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