using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Yarn.Unity;
using System.Collections.Generic;

public class SpriteBehavior : MonoBehaviour
{
    [Header("Main Portrait References")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private RectTransform portraitContainer;

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
    private string currentCharacterName;
    private Dictionary<string, Image> activePortraits = new Dictionary<string, Image>();

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
        portraitContainer.anchoredPosition = data.anchoredPosition;
        portraitImage.preserveAspect = true;
        portraitImage.SetNativeSize();
        portraitContainer.localScale = Vector3.one * sizeMultiplier;
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
        portraitContainer.anchoredPosition = startPos;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            c.a = t;
            portraitImage.color = c;
            portraitContainer.anchoredPosition = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        c.a = 1f;
        portraitImage.color = c;
        portraitContainer.anchoredPosition = endPos;
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
        Vector2 originalPos = portraitContainer.anchoredPosition;
        Vector2 endPos = originalPos - new Vector2(moveOffset, 0);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            c.a = 1f - t;
            portraitImage.color = c;
            portraitContainer.anchoredPosition = Vector2.Lerp(originalPos, endPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        c.a = 0f;
        portraitImage.color = c;
        portraitImage.enabled = false;
        portraitContainer.anchoredPosition = originalPos;
        currentTransition = null;
    }

    private void HideChibiInstantly()
    {
        chibiImage.enabled = false;
    }


    // <<showCharacter "Luna" at 200 -50>>
    [YarnCommand("showCharacter")]
    public void ShowCharacterAt(string characterName, string atKeyword, float x, float y)
    {
        var data = database.GetCharacter(characterName);
        if (data == null) return;

        // Remove if already exists
        if (activePortraits.TryGetValue(characterName, out Image existing))
        {
            Destroy(existing.gameObject);
            activePortraits.Remove(characterName);
        }

        // Create new
        GameObject go = Instantiate(portraitPrefab, portraitContainer);
        Image img = go.GetComponent<Image>();
        img.sprite = data.portrait;
        img.preserveAspect = true;
        img.SetNativeSize();
        img.color = new Color(1, 1, 1, 0); // start transparent
        img.rectTransform.anchoredPosition = new Vector2(x, y) + new Vector2(moveOffset, 0);
        img.transform.localScale = Vector3.one * sizeMultiplier;

        activePortraits[characterName] = img;
        StartCoroutine(FadeInAndMove(img, new Vector2(x, y)));
    }

    // <<showChibi "Kai" at -150 -60>>
    [YarnCommand("showChibi")]
    public void ShowChibiAt(string characterName, string atKeyword, float x, float y)
    {
        var data = database.GetCharacter(characterName);
        if (data == null || data.chibiPortrait == null) return;

        if (activePortraits.TryGetValue(characterName, out Image existing))
        {
            Destroy(existing.gameObject);
            activePortraits.Remove(characterName);
        }

        GameObject go = Instantiate(portraitPrefab, portraitContainer);
        Image img = go.GetComponent<Image>();
        img.sprite = data.chibiPortrait;
        img.preserveAspect = true;
        img.SetNativeSize();
        img.color = new Color(1, 1, 1, 0);
        img.rectTransform.anchoredPosition = new Vector2(x, y) + new Vector2(moveOffset, 0);
        img.transform.localScale = Vector3.one * chibiSizeMultiplier;

        activePortraits[characterName] = img;
        StartCoroutine(FadeInAndMove(img, new Vector2(x, y)));
    }

    // <<clearCharacters>>
    [YarnCommand("clearCharacters")]
    public void ClearCharacters()
    {
        foreach (var img in activePortraits.Values)
            Destroy(img.gameObject);
        activePortraits.Clear();
    }

    // Reuse your fade logic for new portraits
    private IEnumerator FadeInAndMove(Image img, Vector2 targetPos)
    {
        float elapsed = 0;
        Color c = img.color;
        Vector2 startPos = img.rectTransform.anchoredPosition;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            c.a = t;
            img.color = c;
            img.rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        c.a = 1;
        img.color = c;
    }


    #region BG

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite defaultBackground;

    [YarnCommand("setBackground")]
    public void SetBackground(Sprite bg = null)
    {
        if (bg == null)
            bg = defaultBackground;

        if (bg == null)
        {
            backgroundImage.enabled = false;
            return;
        }

        backgroundImage.sprite = bg;
        backgroundImage.enabled = true;
    }

    [YarnCommand("hideBackground")]
    public void HideBackground()
    {
        backgroundImage.enabled = false;
    }

    #endregion
}
