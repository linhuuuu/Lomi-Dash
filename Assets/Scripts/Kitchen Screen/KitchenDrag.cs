using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class KitchenDrag : MonoBehaviour
{
    [Header("Positionals")]
    [SerializeField] private Vector3 leftBounds;
    [SerializeField] private Vector3 rightBounds;
    private Vector3 originalPos, worldPos, targetPos, dragStartOffset;

    [Header("Dragging References")]
    [SerializeField] private LayerMask interactable;
    [SerializeField] private bool isKitchenFocus, isDragging;
    private int activeTouchId;
    private Camera mainCam;

    [Header("UI")]
    [SerializeField] private Canvas morningCanvas;
    [SerializeField] private Canvas kitchenCanvas;

    void Awake()
    {
        mainCam = CameraManager.cam.mainCam;
        originalPos = transform.position;
        isKitchenFocus = false;
        isDragging = false;
        activeTouchId = -1;
        kitchenCanvas.enabled = false;
    }

    void Update()
    {
        if (!isKitchenFocus) return;

        if (Input.touchSupported && Input.touchCount > 0)
            HandleTouch();

        else
            HandleMouse();
    }

    #region  Dragging Logic
    private void HandleTouch()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (isDragging) break;

                    if (!IsInputBlocked(touch.position))
                    {
                        isDragging = true;
                        activeTouchId = touch.fingerId;

                        worldPos = GetWorldPosition(touch.position);
                        dragStartOffset = transform.position - worldPos;
                    }
                    break;

                case TouchPhase.Moved:
                    if (isDragging && touch.fingerId == activeTouchId)
                    {
                        worldPos = GetWorldPosition(touch.position);
                        targetPos = worldPos + dragStartOffset;

                        transform.position = ClampPosition(targetPos);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (touch.fingerId == activeTouchId)
                        EndDrag();
                    break;
            }
        }
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsInputBlocked(Input.mousePosition))
            {
                isDragging = true;

                worldPos = GetWorldPosition(Input.mousePosition);
                dragStartOffset = transform.position - worldPos;
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            worldPos = GetWorldPosition(Input.mousePosition);
            targetPos = worldPos + dragStartOffset;

            transform.position = ClampPosition(targetPos);
        }

        if (Input.GetMouseButtonUp(0))
            EndDrag();
    }

    #endregion
    #region  Dragging Helpers

    private bool IsInputBlocked(Vector3 worldPos)
    {
        if (IsOverUI(worldPos))
        {
            Debug.Log("ya");
            return true;
        }


        Ray ray = mainCam.ScreenPointToRay(worldPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, interactable))
            return true;
        return false;
    }

    private bool IsOverUI(Vector3 worldPos)
    {
        if (EventSystem.current == null) return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = worldPos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        for (int i = 0; i < results.Count; i++)
            if (results[i].module is GraphicRaycaster)
                return true;
        return false;
    }

    private Vector3 GetWorldPosition(Vector3 worldPos)
    {
        worldPos.z = mainCam.WorldToScreenPoint(new Vector3(0, 0, transform.position.z)).z;
        return mainCam.ScreenToWorldPoint(worldPos);
    }

    private Vector3 ClampPosition(Vector3 target)
    {
        float clampedX = Mathf.Clamp(target.x, rightBounds.x, leftBounds.x);
        float t = Mathf.Clamp01(Mathf.InverseLerp(leftBounds.x, rightBounds.x, clampedX));
        float z = Mathf.Lerp(leftBounds.z, rightBounds.z, t);

        return new Vector3(clampedX, transform.position.y, z);
    }

    private void EndDrag()
    {
        if (isDragging)
        {
            isDragging = false;
            activeTouchId = -1;
        }
    }

    #endregion
    #region Kitchen Control

    public void ToggleKitchen()
    {
        isKitchenFocus = !isKitchenFocus;

        if (isKitchenFocus)
        {
            kitchenCanvas.enabled = true;
            morningCanvas.enabled = false;
            LeanTween.move(gameObject, leftBounds, 0.2f).setEaseInSine();
        }

        else
        {
            kitchenCanvas.enabled = false;
            morningCanvas.enabled = true;
            LeanTween.move(gameObject, originalPos, 0.2f).setEaseOutSine();
        }

    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) EndDrag();
    }

    private void OnApplicationPause(bool pauseStatus) => EndDrag();

    #endregion
}