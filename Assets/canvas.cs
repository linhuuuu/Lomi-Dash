using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class FixedAspectRatio : MonoBehaviour
{
    [SerializeField] private float targetAspect = 16f / 9f; // Portrait

    private CanvasScaler canvasScaler;
    private RectTransform canvasRect;

    void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();
        canvasRect = GetComponent<RectTransform>();

        ApplyLetterbox();
    }

    void Update()
    {
        ApplyLetterbox(); // Recalculate on resize
    }

    void ApplyLetterbox()
    {
        float currentAspect = canvasRect.rect.width / canvasRect.rect.height;
        float scaleRatio = currentAspect / targetAspect;

        if (scaleRatio >= 1f)
        {
            // Screen is wider → add side bars
            float newWidth = canvasRect.rect.height * targetAspect;
            float scaleFactor = newWidth / canvasRect.rect.width;

            canvasScaler.matchWidthOrHeight = 0f; // Scale based on height
        }
        else
        {
            // Screen is taller → let content handle (or center)
            canvasScaler.matchWidthOrHeight = 0f;
        }
    }
}