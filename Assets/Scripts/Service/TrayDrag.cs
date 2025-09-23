using PCG;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;

public class TrayDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 originalPos;
    [SerializeField] private PrepTray tray;
    private LayerMask tableMask;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        tableMask = 1 << 9;

    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPos = transform.localPosition;
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
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, tableMask))
        {
            if(Debug.isDebugBuild) Debug.Log("Dropped on table: " + hit.collider.name);

            //Get Customer Group
            TableDropZone table = hit.collider.GetComponent<TableDropZone>();
            if (table.occupied == false)
            {
                Debug.Log(table.occupied);
                transform.localPosition = originalPos;
                return;
            }
            CustomerGroup group = table.transform.GetComponentInChildren<CustomerGroup>();
            
            //Get Orders
            OrderNode order = RoundManager.roundManager.orders[group.orderID].order;
            OrderNode cookedOrder = tray.CompleteTray();

            //Evaluate
            float score = order.Evaluate(cookedOrder);
            Debug.Log(score);

            //Clear Out Tray
            tray.ClearTray();
        }
        transform.localPosition = originalPos;
    }

}