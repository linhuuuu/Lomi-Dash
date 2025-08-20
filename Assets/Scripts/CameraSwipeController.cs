using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    [Header("Drag")]
    [SerializeField] private float dragSpeed = 2.0f;
    [SerializeField] private bool useSmoothing = true;
    [SerializeField] private float smoothTime = 0.1f;

    [Header("Bounds (World Space)")]
    [SerializeField] private Vector2 minBounds = new Vector2(-10f, -5f);  // left, bottom
    [SerializeField] private Vector2 maxBounds = new Vector2(10f, 5f);   // right, top

    private Vector3 dragOrigin;
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        targetPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        // Calculate drag delta in viewport space
        Vector3 pos = cam.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
        Vector3 move = new Vector3(pos.x * dragSpeed, 0, 0); // Z = 0 → no zoom

        // Accumulate movement
        targetPosition = transform.position + move;

        // Clamp to bounds
        float clampedX = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);

        Vector3 clampedTarget = new Vector3(clampedX, clampedY, transform.position.z);

        if (useSmoothing)
        {
            // Smooth damp for staggered, buttery movement
            transform.position = Vector3.SmoothDamp(transform.position, clampedTarget, ref velocity, smoothTime);
        }
        else
        {
            transform.position = clampedTarget;
        }
    }
}