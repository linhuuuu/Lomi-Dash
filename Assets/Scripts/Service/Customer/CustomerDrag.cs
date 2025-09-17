using UnityEngine;
public class CustomerDrag : MonoBehaviour
{
    private Vector3 originalPosition;
    private Camera mainCamera;
    private float zOffset;
    private SpriteRenderer spriteRenderer;
    [HideInInspector] public bool Snapped = false;
    public CustomerData customerData { set; get; }

    private void Awake()
    {
        mainCamera = CameraManager.cam.mainCam;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void InitCustomer()
    {   
        spriteRenderer.sprite = customerData.standingSprite;
        this.name = customerData.name;
    }

    private void OnMouseDown()
    {
        if (Snapped) return;

        originalPosition = transform.position;
        zOffset = mainCamera.WorldToScreenPoint(transform.position).z;

        if (customerData.pickUpSprite != null)
            spriteRenderer.sprite = customerData.pickUpSprite;
    }

    private void OnMouseDrag()
    {
        if (Snapped) return;

        Vector3 screenPos = Input.mousePosition;
        screenPos.z = zOffset;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);

        transform.position = worldPos;
    }

    private void OnMouseUp()
    {
        if (Snapped) return;

        // Check all tables in the scene
        TableDropZone[] tables = FindObjectsOfType<TableDropZone>();
        foreach (TableDropZone table in tables)
        {
            if (table.IsInsideTable(transform.position))
            {
                if (table.TrySeatCustomer(this))
                {
                    return; // seated successfully
                }
            }
        }

        // If no valid table, reset
        transform.position = originalPosition;
        if (customerData.standingSprite != null)
            spriteRenderer.sprite = customerData.standingSprite;
    }

    public void SitDown()
    {
        Snapped = true;
        if (customerData.sittingSprite != null)
            spriteRenderer.sprite = customerData.sittingSprite;
    }
}
