using PCG;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrayDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 originalPos;
    [SerializeField] private PrepTray tray;
    [SerializeField] private LayerMask mask;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPos = transform.localPosition;
        CameraDragZoomControl.isCameraDraggingEnabled = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, canvas.worldCamera,
        out Vector3 worldPoint))
            transform.position = worldPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Ray ray = CameraManager.cam.mainCam.ScreenPointToRay(eventData.position);

        //Get CustomerGroup Component from Table or OrderPrompt
        CustomerGroup group = null;
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, mask))
        {
            if (hit.collider.TryGetComponent(out TableDropZone table))
            {
                if (table.occupied == false)
                {
                    transform.localPosition = originalPos;
                    return;
                }
                group = table.transform.GetComponentInChildren<CustomerGroup>();
            }

            if (hit.collider.TryGetComponent(out OrderPrompt prompt))
                group = RoundManager.roundManager.orders[prompt.orderIndex].customers;

            if (group.prompt.isOrderTaken == false)
            {
                transform.localPosition = originalPos;
                return;
            }

            //Get Orders
            OrderNode order = RoundManager.roundManager.orders[group.orderID].order;
            OrderNode cookedOrder = tray.CompleteTray();

            //Evaluate
            cookedOrder.weight = order.Evaluate(cookedOrder);
            Debug.Log(cookedOrder.weight);

            //Leave
            RoundManager.roundManager.finishedOrders[group.orderID] = cookedOrder;
            RoundManager.roundManager.OnCustomerGroupLeaveDined(group);
            group.RemoveAll();

            //Clear Out Tray
            tray.ClearTray();
        }
        transform.localPosition = originalPos;
                CameraDragZoomControl.isCameraDraggingEnabled = true;
    }

}