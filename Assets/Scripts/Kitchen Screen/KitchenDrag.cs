using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class KitchenDrag : MonoBehaviour
{
    [Header("Position References")]
    [SerializeField] private Vector3 leftBounds;
    [SerializeField] private Vector3 rightBounds;
    private Vector3 originalPos, worldPos, targetPos, dragStartOffset;

    [Header("Dragging References")]
    [SerializeField] private LayerMask interactable;
    [SerializeField] private bool isKitchenFocus, isDragging;
    private int activeTouchId;

    [Header("Zoom")]
    [SerializeField] private float minOrtho = 4f;
    [SerializeField] private float maxOrtho = 5f;

    //Object References
    private Canvas mainCanvas;
    private Canvas kitchenCanvas;

    private Camera mainCam;
    [field: SerializeField] public Slider zoomSlider { set; get; }

    public static KitchenDrag Instance;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        mainCam = CameraManager.cam.mainCam;
        mainCanvas = MainScreenManager.main.activeScreen;
        kitchenCanvas = MainScreenManager.main.kitchenScreen;

        originalPos = transform.position;
        isKitchenFocus = false;
        isDragging = false;
        activeTouchId = -1;

        zoomSlider.onValueChanged.AddListener((float value) => SetCameraZoom(value));

    }

    void Update()
    {
        if (!isKitchenFocus) return;

        if (Input.touchSupported && Input.touchCount > 0)
            HandleTouch();
        else
            HandleMouse();
    }

    #region Dragging Logic 
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
    #region Helpers 

    private bool IsInputBlocked(Vector3 screenPos)
    {
        if (UIUtils.IsPointerOverUI()) return true;
        Ray ray = mainCam.ScreenPointToRay(screenPos);
        return Physics.Raycast(ray, out RaycastHit hit, 1000f, interactable);
    }

    private Vector3 GetWorldPosition(Vector3 screenPos)
    {
        screenPos.z = mainCam.WorldToScreenPoint(transform.position).z;
        return mainCam.ScreenToWorldPoint(screenPos);
    }

    private Vector3 ClampPosition(Vector3 target)
    {
        float clampedX = Mathf.Clamp(target.x, rightBounds.x, leftBounds.x);
        float t = Mathf.Clamp01(Mathf.InverseLerp(leftBounds.x, rightBounds.x, clampedX));
        float z = Mathf.Lerp(leftBounds.z, rightBounds.z, t);
        return new Vector3(clampedX, transform.position.y, z);
    }

    public void NudgeKitchen(float direction)
    {
        if (!isKitchenFocus) return;

        float nudgeAmount = 0.2f;
        Vector3 newPos = transform.position + Vector3.right * direction * nudgeAmount;
        transform.position = ClampPosition(newPos);
    }

    public void SetCameraZoom(float val)
    {
        if (mainCam == null || !isKitchenFocus) return;

        float orthoSize = Mathf.Lerp(maxOrtho, minOrtho, val);
        mainCam.orthographicSize = orthoSize;
    }

    private void EndDrag()
    {
        isDragging = false;
        activeTouchId = -1;
    }

    #endregion
    #region Kitchen Control

    public void ToggleKitchen()
    {
        isKitchenFocus = !isKitchenFocus;

        if (isKitchenFocus)
        {
            CameraDragZoomControl.isCameraDraggingEnabled = false;
            kitchenCanvas.enabled = true;
            mainCanvas.enabled = false;
            LeanTween.move(gameObject, leftBounds, 0.2f).setEaseInSine();
            mainCam.orthographicSize = maxOrtho;

            // Optional: reset zoom to default when opening
            if (zoomSlider != null)
                zoomSlider.value = 0; // middle
        }
        else
        {
            CameraDragZoomControl.isCameraDraggingEnabled = true;
            kitchenCanvas.enabled = false;
            mainCanvas.enabled = true;
            LeanTween.move(gameObject, originalPos, 0.2f).setEaseOutSine();
        }
    }

    private void OnApplicationFocus(bool hasFocus) { if (!hasFocus) EndDrag(); }
    private void OnApplicationPause(bool pauseStatus) => EndDrag();
    #endregion
}

#region UIUtils
public static class UIUtils
{
    private static readonly List<RaycastResult> _results = new List<RaycastResult>();

    public static bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        var eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        _results.Clear();
        EventSystem.current.RaycastAll(eventData, _results);

        for (int i = 0; i < _results.Count; i++)
        {
            if (_results[i].module is GraphicRaycaster)
                return true;
        }
        return false;
    }
}
#endregion