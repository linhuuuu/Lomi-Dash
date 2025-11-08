using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Yarn.Unity;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;

public class SpriteBehavior : MonoBehaviour
{

    [Header("Main Portrait References")]
    [SerializeField] private GameObject portraitPrefab;
    [SerializeField] private RectTransform portraitContainer;
    [SerializeField] private TextMeshProUGUI characterName;

    [Header("Chibi Portrait References")]
    [SerializeField] private Image chibiImage;

    [Header("Background References")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite defaultBackground;

    [Header("Data Source")]
    [SerializeField] private CharacterDatabase database;
    [SerializeField] private BackgroundDatabase backgroundDB;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float moveOffset = 30f;
    [SerializeField, Range(0.5f, 1f)] private float sizeMultiplier = 0.9f;

    [Header("Visual States")]
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    [SerializeField] private DialogueRunner runner;

    //Curent Data
    private string currentSpeaker = null;
    private Dictionary<string, Image> activePortraits = new();


    #region Runtime Control

    public void HidePortraits()
    {
        FadeOutAllMainPortraits();
        HideChibi();
        currentSpeaker = null;
    }

    public void ShowPortraits()
    {
        FadeInAllMainPortraits();
        if (!string.IsNullOrEmpty(currentSpeaker))
            UpdateChibi(currentSpeaker);
    }

    public void SetActiveSpeaker(string speakerName)
    {
        currentSpeaker = speakerName;


        if (currentSpeaker == "Protagonist")
        {
            characterName.text = DataManager.data.playerData.playerName;
        }

        if (currentSpeaker == "Narrator")
        {
            characterName.text = "";
            currentSpeaker = "";
        }

        if (currentSpeaker.Contains("_"))
            characterName.text = currentSpeaker.Replace("_", " ");

        if (currentSpeaker.EndsWith("?"))
        {
            currentSpeaker = currentSpeaker.TrimEnd("?");
            characterName.text = $"???";
        }

        UpdatePortraitHighlights();

        if (!string.IsNullOrEmpty(currentSpeaker))
            UpdateChibi(currentSpeaker);
        else
            HideChibi();
    }

    #endregion
    #region Command Portrait Control

    [YarnCommand("setPlayerName")]
    public void SetPlayerName()
    {
        runner?.VariableStorage.SetValue("$player", DataManager.data.playerData.playerName);
    }

    [YarnCommand("showCharacter")]
    public void ShowCharacterAt(string characterName, string atKeyword, float x, float y)
    {
        var data = database.GetCharacter(characterName);
        if (data == null) return;

        currentSpeaker = characterName;

        if (!activePortraits.TryGetValue(characterName, out Image img))
        {
            GameObject go = Instantiate(portraitPrefab, portraitContainer);
            img = go.GetComponent<Image>();
            img.sprite = data.defaultSprite;
            img.preserveAspect = true;
            img.SetNativeSize();
            img.transform.localScale = Vector3.one * 2;
            img.color = new Color(1, 1, 1, 0);
            img.rectTransform.anchoredPosition = new Vector2(x, y) + new Vector2(moveOffset, 0);
            activePortraits[characterName] = img;

            FadeInAndMove(img, new Vector2(x, y));
        }
        else
        {
            img.rectTransform.anchoredPosition = new Vector2(x, y);
        }

        UpdateChibi(characterName);
        UpdatePortraitHighlights();
    }

    [YarnCommand("changeCharacterSprite")]
    public void ChangeCharacterSprite(string character, string emotion)
    {
        var data = database.GetCharacter(character);
        activePortraits.TryGetValue(character, out Image img);
        var foundCharacter = data.sprites.Find(c => c.id == emotion);

        if (foundCharacter != null)
        {
            img.sprite = foundCharacter.sprite;
        }
    }

    [YarnCommand("clearCharacters")]
    public void ClearCharacters(string[] targets)
    {
        if (targets == null || targets.Length == 0)
        {
            Debug.LogWarning("No target provided to <<clearCharacters>>");
            return;
        }

        string command = targets[0].Trim();

        if (command.Equals("All", System.StringComparison.OrdinalIgnoreCase))
        {
            FadeOutAllMainPortraits();
            activePortraits.Clear();
            HideChibi();
            currentSpeaker = null;
            return;
        }

        // Handle one or more specific characters
        foreach (string name in targets)
        {
            string cleanName = name.Trim();
            if (activePortraits.TryGetValue(cleanName, out Image img))
            {
                StartCoroutine(FadeOutAndRemove(img));
                activePortraits.Remove(cleanName);

                if (currentSpeaker == cleanName)
                {
                    currentSpeaker = null;
                    HideChibi();
                }
            }
        }

        UpdatePortraitHighlights();
    }

    [YarnCommand("moveCharacter")]
    public void MoveCharacterTo(string characterName, string toKeyword, float x, float y)
    {
        if (activePortraits.TryGetValue(characterName, out Image img))
        {
            RectTransform rt = img.rectTransform;

            LeanTween.cancel(rt.gameObject);

            LeanTween.value(rt.gameObject,
                (Vector2 val) => { rt.anchoredPosition = val; },
                rt.anchoredPosition,
                new Vector2(x, y),
                fadeDuration)
                .setEase(LeanTweenType.easeInOutSine);
        }
    }

    [YarnCommand("overridePortraitHighlight")]
    public void OverridePortraitHighlight(string[] characters)
    {
        if (activePortraits.Count == 0) return;

        if (characters[0] == "All")
        {
            foreach (var kvp in activePortraits)
                kvp.Value.color = activeColor;
            return;
        }

        if (characters[0] == "None")
        {
            foreach (var kvp in activePortraits)
                kvp.Value.color = inactiveColor;
            return;
        }

        foreach (var kvp in activePortraits)
        {
            string key = kvp.Key.Replace(" ", "_");
            bool isActive = System.Array.Exists(characters, c => c == key);
            kvp.Value.color = isActive ? activeColor : inactiveColor;
        }
    }

    #endregion
    #region  Portrait Helpers

    private void FadeOutAllMainPortraits()
    {
        foreach (var img in activePortraits.Values)
        {
            if (img != null)
            {
                LeanTween.cancel(img.rectTransform);
                LeanTween.alpha(img.rectTransform, 0f, fadeDuration);
            }
        }
    }
    private void FadeInAllMainPortraits()
    {
        foreach (var img in activePortraits.Values)
        {
            if (img != null)
            {
                Color c = img.color;
                c.a = 0f;
                img.color = c;
                img.enabled = true;

                LeanTween.cancel(img.rectTransform);
                LeanTween.alpha(img.rectTransform, 1f, fadeDuration);
            }
        }
    }

    private void FadeInAndMove(Image img, Vector2 targetPos)
    {
        if (img == null) return;

        LeanTween.cancel(img.rectTransform);

        Color color = img.color;
        color.a = 0f;
        img.color = color;
        img.rectTransform.anchoredPosition += new Vector2(moveOffset, 0);

        LeanTween.alpha(img.rectTransform, 1f, fadeDuration);
        LeanTween.value(img.rectTransform.gameObject,
            (float t) => { },
            0f, 1f, fadeDuration)
            .setOnUpdate((float t) =>
            {
                float easeT = Mathf.SmoothStep(0, 1, t);
                img.rectTransform.anchoredPosition = Vector2.Lerp(
                    img.rectTransform.anchoredPosition,
                    targetPos,
                    easeT
                );
            });
    }

    private IEnumerator FadeOutAndRemove(Image img)
    {
        float elapsed = 0f;
        Vector2 startPos = img.rectTransform.anchoredPosition;
        Vector2 endPos = startPos - new Vector2(moveOffset, 0);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float easeT = Mathf.SmoothStep(0, 1, t);

            Color c = img.color;
            c.a = 1f - t;
            img.color = c;
            img.rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, easeT);
            yield return null;
        }

        img.enabled = false;
    }

    private void UpdatePortraitHighlights()
    {
        if (activePortraits.Count == 0) return;

        foreach (var kvp in activePortraits)
        {
            bool isActive = kvp.Key == currentSpeaker;
            kvp.Value.color = isActive ? activeColor : inactiveColor;
        }
    }

    #endregion
    #region  Chibi

    private void UpdateChibi(string characterName)
    {
        var data = database.GetCharacter(characterName);
        if (data != null && data.chibiPortrait != null)
        {
            chibiImage.sprite = data.chibiPortrait;
            // chibiImage.SetNativeSize();
            chibiImage.preserveAspect = true;
            chibiImage.enabled = true;
        }
        else
        {
            chibiImage.enabled = false;
        }
    }

    [YarnCommand("showChibi")]
    public void ShowChibi(string characterName)
    {
        currentSpeaker = characterName;
        UpdateChibi(characterName);
    }

    [YarnCommand("hideChibi")]
    public void HideChibi()
    {
        chibiImage.enabled = false;
    }

    #endregion
    #region Background

    [YarnCommand("setBackground")]
    public void SetBackground(string bg)
    {
        if (string.IsNullOrEmpty(bg) || bg == null)
        {
            backgroundImage.enabled = false;
            return;
        }

        if (bg == "Default")
        {
            backgroundImage.sprite = defaultBackground;
            backgroundImage.enabled = true;
            return;
        }

        var entry = backgroundDB.bgs.Find(c => c.name == bg);
        if (entry != null && entry.image != null)
        {
            backgroundImage.sprite = entry.image;
            backgroundImage.enabled = true;
        }
        else
        {
            backgroundImage.enabled = false;

        }
    }

    [YarnCommand("hideBackground")]
    public void HideBackground()
    {
        backgroundImage.enabled = false;
    }

    #endregion
}