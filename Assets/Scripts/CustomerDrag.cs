using UnityEngine;

public class CustomerDrag : MonoBehaviour
{
    private Vector3 originalPosition;
    private Camera mainCamera;
    private float zOffset;
    private SpriteRenderer spriteRenderer;

    [HideInInspector] public bool Snapped = false;

    [Header("Sprites")]
    public Sprite defaultSprite;
    public Sprite draggingSprite;
    public Sprite sittingSprite;

    private void Awake()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = defaultSprite;
    }

    private void OnMouseDown()
    {
        if (Snapped) return;

        originalPosition = transform.position;
        zOffset = mainCamera.WorldToScreenPoint(transform.position).z;

        if (draggingSprite != null)
            spriteRenderer.sprite = draggingSprite;
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
        if (defaultSprite != null)
            spriteRenderer.sprite = defaultSprite;
    }

    public void SitDown()
    {
        Snapped = true;
        if (sittingSprite != null)
            spriteRenderer.sprite = sittingSprite;
    }
}
