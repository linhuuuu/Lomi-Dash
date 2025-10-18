using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class AnimResults : MonoBehaviour
{
    [Header("Modals")]
    public GameObject[] modals; // Assign in inspector: [Stats, Leaderboard, ...]

    [Header("Navigation")]
    public Button[] toggleButtons; // Must match modals length
    public float slideDuration = 0.4f;
    // public AnimationCurve slideEase = LeanTween.easeInOutQuad();

    private int currentModalIndex = 0;
    private RectTransform container;
    private Vector2 modalSize;

    void Start()
    {
        container = GetComponent<RectTransform>();
        if (modals.Length == 0) return;

        // Get size from first modal
        modalSize = modals[0].GetComponent<RectTransform>().sizeDelta;

        // Deactivate all, activate first
        for (int i = 0; i < modals.Length; i++)
        {
            modals[i].SetActive(i == 0);
            if (toggleButtons.Length > i) toggleButtons[i].onClick.AddListener(() => SwitchTo(i));
        }

        // Start auto sequence
        StartCoroutine(StartSequence());
    }

    public IEnumerator StartSequence()
    {
        yield return new WaitForSeconds(1f);
        
        // // Animate stats content (you'll implement this inside StatsModal script)
        // if (modals[0].TryGetComponent(out StatsModal stats))
        //     yield return stats.PlayReveal();

        // Then auto-switch to next modal (leaderboard)
        if (modals.Length > 1)
            yield return SwitchTo(1);
    }

    public IEnumerator SwitchTo(int targetIndex)
    {
        if (targetIndex < 0 || targetIndex >= modals.Length || targetIndex == currentModalIndex)
            yield break;

        int prevIndex = currentModalIndex;
        currentModalIndex = targetIndex;

        // Activate target modal immediately (but it's off-screen)
        modals[targetIndex].SetActive(true);

        // Direction: +1 = slide left (new modal comes from right), -1 = slide right
        float direction = targetIndex > prevIndex ? -1f : 1f;

        // Reset positions
        Vector2 prevPos = new Vector2(direction * modalSize.x, 0); // Current modal slides out
        Vector2 targetPos = new Vector2(-direction * modalSize.x, 0); // Target modal starts off-screen

        modals[prevIndex].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        modals[targetIndex].GetComponent<RectTransform>().anchoredPosition = targetPos;

        // // Tween both modals
        // LeanTween.moveLocalX(modals[prevIndex], prevPos.x, slideDuration).setEase(slideEase);
        // LeanTween.moveLocalX(modals[targetIndex], 0, slideDuration).setEase(slideEase);

        yield return new WaitForSeconds(slideDuration);

        // Deactivate old modal
        modals[prevIndex].SetActive(false);
    }
}
