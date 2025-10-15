using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpriteBehavior : MonoBehaviour
{
    [Header("Main Portrait References")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private RectTransform portraitTransform;

    [Header("Chibi Portrait References")]
    [SerializeField] private Image chibiImage;
    [SerializeField] private RectTransform chibiTransform;

    [Header("Data Source")]
    [SerializeField] private CharacterDatabase database;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float moveOffset = 30f;
    [SerializeField, Range(0.5f, 1f)] private float sizeMultiplier = 0.9f; // scale for main portrait
    [SerializeField, Range(0.3f, 1f)] private float chibiSizeMultiplier = 0.8f; // separate chibi scale

    private Coroutine currentTransition;
    private string currentCharacterName; // 👈 track who’s showing now

    public void ShowCharacter(string characterName)
    {
        var data = database.GetCharacter(characterName);
        if (data == null)
        {
            Debug.LogWarning($"Character '{characterName}' not found in database!");
            return;
        }

        // 👇 If it's the same character, skip fade/transition
        if (currentCharacterName == characterName)
        {
            UpdateChibi(data);
            return;
        }

        // 👇 Otherwise, transition to new one
        if (currentTransition != null)
            StopCoroutine(currentTransition);

        currentTransition = StartCoroutine(TransitionToCharacter(data));
        currentCharacterName = characterName; // remember who’s active
    }

    public void HidePortraits()
    {
        if (currentTransition != null)
            StopCoroutine(currentTransition);

        currentCharacterName = null; // reset
        currentTransition = StartCoroutine(FadeOutMainOnly());
        HideChibiInstantly();
    }

    private IEnumerator TransitionToCharacter(CharacterDatabase.CharacterData data)
    {
        // Fade out main portrait only if something is visible
        if (portraitImage.sprite != null && portraitImage.enabled)
            yield return FadeOutMainOnly();

        // --- MAIN PORTRAIT ---
        portraitImage.sprite = data.portrait;
        portraitTransform.anchoredPosition = data.anchoredPosition;
        portraitImage.preserveAspect = true;
        portraitImage.SetNativeSize();
        portraitTransform.localScale = Vector3.one * sizeMultiplier;
        portraitImage.enabled = true;

        // --- CHIBI PORTRAIT ---
        UpdateChibi(data);

        // --- MAIN PORTRAIT TRANSITION ---
        Vector2 startPos = data.anchoredPosition + new Vector2(moveOffset, 0);
        Vector2 endPos = data.anchoredPosition;

        float elapsed = 0f;
        Color c = portraitImage.color;
        c.a = 0f;
        portraitImage.color = c;
        portraitTransform.anchoredPosition = startPos;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            c.a = t;
            portraitImage.color = c;
            portraitTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        c.a = 1f;
        portraitImage.color = c;
        portraitTransform.anchoredPosition = endPos;
        currentTransition = null;
    }

    private void UpdateChibi(CharacterDatabase.CharacterData data)
    {
        if (data.chibiPortrait != null)
        {
            chibiImage.sprite = data.chibiPortrait;
            chibiImage.preserveAspect = true;
            chibiImage.SetNativeSize();
            chibiTransform.localScale = Vector3.one * chibiSizeMultiplier;
            chibiImage.enabled = true;
        }
        else
        {
            chibiImage.enabled = false;
        }
    }

    private IEnumerator FadeOutMainOnly()
    {
        float elapsed = 0f;
        Color c = portraitImage.color;
        Vector2 originalPos = portraitTransform.anchoredPosition;
        Vector2 endPos = originalPos - new Vector2(moveOffset, 0);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            c.a = 1f - t;
            portraitImage.color = c;
            portraitTransform.anchoredPosition = Vector2.Lerp(originalPos, endPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        c.a = 0f;
        portraitImage.color = c;
        portraitImage.enabled = false;
        portraitTransform.anchoredPosition = originalPos;
        currentTransition = null;
    }

    private void HideChibiInstantly()
    {
        chibiImage.enabled = false;
    }
}
