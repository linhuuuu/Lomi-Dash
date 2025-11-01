using UnityEngine;
using System.Collections;

public class IngredientFlipper : MonoBehaviour
{
    [Header("Flip Settings")]
    [SerializeField] private float decayRate = 2f;                 // How fast flips slow down
    [SerializeField] private Vector2 maxWobbleOffset = new Vector2(0.1f, 0.05f);

    [Header("Angular Variation")]
    [SerializeField] private float maxAngleOffset = 30f;           // ± degrees added to flip rotation

    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;

    private Coroutine flipRoutine;

    void Start()
    {
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;
    }

    /// <summary>
    /// Trigger flip with default intensity
    /// </summary>
    public void OnStir()
    {
        if (flipRoutine != null)
            StopCoroutine(flipRoutine);

        flipRoutine = StartCoroutine(OscillateAndSettle());
    }

    /// <summary>
    /// Trigger flip with custom intensity (future input support)
    /// </summary>
    public void OnStir(float intensity)
    {
        float rate = Mathf.Lerp(4f, 10f, intensity);
        if (flipRoutine != null)
            StopCoroutine(flipRoutine);

        flipRoutine = StartCoroutine(OscillateAndSettle(rate));
    }

    /// <summary>
    /// Core: Instantly snap between flipped states with decaying angular offset
    /// Simulates food tumbling in oil after a stir
    /// </summary>
    private IEnumerator OscillateAndSettle(float startRate = 6f)
    {
        float currentRate = startRate;
        bool isFlipped = false;

        // Reset to base state
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        transform.localPosition = originalLocalPos;

        while (currentRate > 0.5f)
        {
            // Base angles: 0 or -180
            float baseAngle = isFlipped ? 0f : -180f;
            isFlipped = !isFlipped;

            // Calculate how "wild" the flip should be based on current speed
            float progress = (currentRate - 0.5f) / (startRate - 0.5f); // 1 = full wildness, 0 = none
            progress = Mathf.Clamp01(progress);

            // Random offset scaled by remaining energy
            float angleOffset = Random.Range(-maxAngleOffset, maxAngleOffset) * progress;
            float finalAngle = baseAngle + angleOffset;

            // Positional wobble for extra life
            Vector3 offset = new Vector3(
                Random.Range(-maxWobbleOffset.x, maxWobbleOffset.x),
                Random.Range(-maxWobbleOffset.y, maxWobbleOffset.y),
                0
            );

            // 🔥 INSTANT SNAP — frame-based, choppy style
            transform.localRotation = Quaternion.Euler(0f, finalAngle, 0f);
            transform.localPosition = originalLocalPos + offset;

            // Wait until next flip
            float delay = 1f / currentRate;
            yield return new WaitForSeconds(delay);

            // Slow down over time
            currentRate -= decayRate * Time.deltaTime;
        }

        // Final reset
        transform.localPosition = originalLocalPos;
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    /// <summary>
    /// Stop flipping immediately (e.g., remove from heat)
    /// </summary>
    public void StopFlipping()
    {
        if (flipRoutine != null)
        {
            StopCoroutine(flipRoutine);
            flipRoutine = null;
        }

        transform.localPosition = originalLocalPos;
        transform.localRotation = originalLocalRot;
    }
}