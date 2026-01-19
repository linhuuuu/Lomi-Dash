using UnityEngine;
using System.Collections;

public class MoveButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform buttonTransform;
    [SerializeField] private GameObject linkedUIPanel;

    [Header("Movement Settings")]
    [SerializeField] private float newX = 100f;
    [SerializeField] private float duration = 0.5f;

    private Vector2 originalPosition;
    private bool isMoving = false;

    public RectTransform ButtonTransform => buttonTransform;
    public Vector2 OriginalPosition => originalPosition;
    public float NewX => newX;
    public GameObject LinkedUIPanel => linkedUIPanel;

    private void Awake()
    {
        if (buttonTransform == null)
            buttonTransform = GetComponent<RectTransform>();

        originalPosition = buttonTransform.anchoredPosition;

        if (linkedUIPanel != null)
            linkedUIPanel.SetActive(false);
    }

    public void MoveTo(float targetX)
    {
        if (!isMoving)
            StartCoroutine(MoveToPosition(targetX));
    }

    private IEnumerator MoveToPosition(float targetX)
    {
        isMoving = true;

        Vector2 start = buttonTransform.anchoredPosition;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            float newPosX = Mathf.Lerp(start.x, targetX, t);
            buttonTransform.anchoredPosition = new Vector2(newPosX, start.y);
            yield return null;
        }

        buttonTransform.anchoredPosition = new Vector2(targetX, start.y);
        isMoving = false;
    }
}
