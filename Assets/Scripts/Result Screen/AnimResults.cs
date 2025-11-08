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
            if (toggleButtons.Length > i)
            {
                int index = i; // Closure capture
                toggleButtons[i].onClick.AddListener(() => StartCoroutine(SwitchTo(index)));
            }
        }

        // Set initial positions
        ResetAllPositions();
        UpdateButtonStates();
    }

    public IEnumerator StartSequence()
    {
        yield return new WaitForSeconds(1f);
        if (modals.Length > 1)
            yield return SwitchTo(1);
    }

    public IEnumerator SwitchTo(int targetIndex)
    {
        if (targetIndex < 0 || targetIndex >= modals.Length || targetIndex == currentModalIndex)
            yield break;

        int prevIndex = currentModalIndex;
        currentModalIndex = targetIndex;

        // Activate target
        modals[targetIndex].SetActive(true);

        // Direction: left (-1) or right (+1)
        float direction = targetIndex > prevIndex ? -1f : 1f;
        Vector2 offset = new Vector2(direction * modalSize.x, 0);

        // Position: current stays, target comes from off-screen
        modals[prevIndex].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        modals[targetIndex].GetComponent<RectTransform>().anchoredPosition = offset;

        // Animate both
        LeanTween.cancel(modals[prevIndex]);
        LeanTween.cancel(modals[targetIndex]);

        LeanTween.value(modals[prevIndex], 
            val => modals[prevIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(val, 0),
            0f, -direction * modalSize.x, slideDuration)
            .setEase(LeanTweenType.easeInOutQuad);

        LeanTween.value(modals[targetIndex],
            val => modals[targetIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(val, 0),
            offset.x, 0f, slideDuration)
            .setEase(LeanTweenType.easeInOutQuad);

        yield return new WaitForSeconds(slideDuration);

        // Finish: hide old
        modals[prevIndex].SetActive(false);

        // Ensure final position
        modals[targetIndex].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        // Update UI
        UpdateButtonStates();
    }

    private void ResetAllPositions()
    {
        for (int i = 0; i < modals.Length; i++)
        {
            var rt = modals[i].GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
        }
    }

    private void UpdateButtonStates()
    {
        for (int i = 0; i < toggleButtons.Length; i++)
        {
            if (i == currentModalIndex)
            {
                toggleButtons[i].interactable = false;
                // Optional: add selected visual (e.g., glow)
            }
            else
            {
                toggleButtons[i].interactable = true;
            }
        }
    }
}