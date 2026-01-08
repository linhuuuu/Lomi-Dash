using UnityEngine;
using System.Collections;

public class JitterEffect : MonoBehaviour
{
    [Header("Choppy Jitter Settings")]
    [SerializeField] private Vector2 maxJitter = new Vector2(0.01f, 0.01f);
    [SerializeField] private float updateRate = 0.2f;
    [SerializeField] private float slowDownDuration = 0.4f;
    [SerializeField] private bool jitterAtStart = false;

    private Coroutine jitterRoutine;

    private SpriteRenderer objSprite;
    private Vector3 originalPos;

    void Start()
    {
        objSprite = GetComponent<SpriteRenderer>();
        originalPos = transform.localPosition;
        
        if (jitterAtStart)
            StartJitter();
    }

    public void StartJitter()
    {
        if (jitterRoutine == null)
            jitterRoutine = StartCoroutine(JitterLoop());
    }

    public void StopJitter()
    {
        if (jitterRoutine != null)
        {
            StopCoroutine(jitterRoutine);
            jitterRoutine = StartCoroutine(SlowDownJitter());
        }
    }

    private IEnumerator SlowDownJitter()
    {
        float elapsed = 0f;
        Vector3 lastOffset = Vector3.zero;

        while (elapsed < slowDownDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slowDownDuration;

            float currentAmplitude = Mathf.Lerp(maxJitter.magnitude, 0f, t);
            float currentRate = Mathf.Lerp(updateRate, updateRate * 3f, t);

            Vector2 offsetDir = Random.insideUnitCircle.normalized;
            Vector3 offset = new Vector3(offsetDir.x, offsetDir.y, 0) * currentAmplitude;
            transform.localPosition = originalPos + offset;

            lastOffset = offset;

            yield return new WaitForSeconds(currentRate);
        }

        transform.localPosition = originalPos;
        jitterRoutine = null;
    }

    private IEnumerator JitterLoop()
    {
        while (true)
        {
            float x = Random.Range(-maxJitter.x, maxJitter.x);
            float y = Random.Range(-maxJitter.y, maxJitter.y);

            transform.localPosition = originalPos + new Vector3(x, y, 0);

            yield return new WaitForSeconds(updateRate);
        }
    }

    public void ToggleJitter()
    {
        if (jitterRoutine == null)
            StartJitter();
        else
            StopJitter();
    }

    public void ChangeSprite(Sprite sprite)
    {
        if (objSprite != null && sprite != null)
        {
            objSprite.sprite = sprite;
        }
    }
}