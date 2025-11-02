using UnityEngine;
using System.Collections;

public class IngredientFlipper : MonoBehaviour
{
    [Header("Flip Settings")]
    [SerializeField] private float decayRate = 2f;        
    [SerializeField] private Vector2 maxWobbleOffset = new Vector2(0.1f, 0.05f);

    [Header("Angular Variation")]
    [SerializeField] private float maxAngleOffset = 30f;   

    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;

    private Coroutine flipRoutine;

    void Start()
    {
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;
    }

    public void OnStir()
    {
        if (flipRoutine != null)
            StopCoroutine(flipRoutine);

        flipRoutine = StartCoroutine(OscillateAndSettle());
    }

    public void OnStir(float intensity)
    {
        float rate = Mathf.Lerp(4f, 10f, intensity);
        if (flipRoutine != null)
            StopCoroutine(flipRoutine);

        flipRoutine = StartCoroutine(OscillateAndSettle(rate));
    }

    private IEnumerator OscillateAndSettle(float startRate = 6f)
    {
        float currentRate = startRate;
        bool isFlipped = false;

        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        transform.localPosition = originalLocalPos;

        while (currentRate > 0.5f)
        {
            float baseAngle = isFlipped ? 0f : -180f;
            isFlipped = !isFlipped;

            float progress = (currentRate - 0.5f) / (startRate - 0.5f); // 1 = full wildness, 0 = none
            progress = Mathf.Clamp01(progress);

            float angleOffset = Random.Range(-maxAngleOffset, maxAngleOffset) * progress;
            float finalAngle = baseAngle + angleOffset;

            Vector3 offset = new Vector3(
                Random.Range(-maxWobbleOffset.x, maxWobbleOffset.x),
                Random.Range(-maxWobbleOffset.y, maxWobbleOffset.y),
                0
            );

            transform.localRotation = Quaternion.Euler(0f, finalAngle, 0f);
            transform.localPosition = originalLocalPos + offset;

            float delay = 1f / currentRate;
            yield return new WaitForSeconds(delay);

            currentRate -= decayRate * Time.deltaTime;
        }

        transform.localPosition = originalLocalPos;
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

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