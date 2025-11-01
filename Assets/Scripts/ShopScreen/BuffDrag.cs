using PCG;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Vector3 originalPos;


    private Image image;
    [SerializeField] private GameObject countObj;
    [SerializeField] private TextMeshProUGUI number;
    [SerializeField] private BuffData buffData;
    [SerializeField] private Canvas canvas;
    [SerializeField] private LayerMask mask;

    void Awake()
    {
        image = GetComponent<Image>();
        image.sprite = buffData.sprite;

        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPos = transform.localPosition;
        countObj.SetActive(false);
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
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.blue, 0.2f);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            if (hit.collider.tag == "Tray")
            {
                RoundManager.roundManager.activeBuffData = buffData;
            }
        }
        transform.localPosition = originalPos;
        countObj.SetActive(true);
    }

}