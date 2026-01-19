using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // for TextMeshPro support

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager instance;

    [Header("Achievement Setup")]
    [SerializeField] private List<Achievement> achievements = new List<Achievement>();

    [Header("Popup Setup")]
    [SerializeField] private GameObject popupPrefab; // ← drag your prefab here
    [SerializeField] private Transform popupParent; // ← assign Canvas here
    [SerializeField] private float popupDuration = 3f;

    // Gameplay counters
    private static int lomiServed = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        InitializeAchievements();
    }

    private void InitializeAchievements()
    {
        if (achievements != null && achievements.Count > 0)
            return;

        achievements = new List<Achievement>();

        achievements.Add(new Achievement(
            "First Lomi!",
            "Place your first Lomi on the tray.",
            (object o) => lomiServed >= 1
        ));

        achievements.Add(new Achievement(
            "Lomi Lover",
            "Serve 5 Lomi dishes.",
            (object o) => lomiServed >= 5
        ));
    }

    void Update()
    {
        CheckAchievementCompletion();
    }

    private void CheckAchievementCompletion()
    {
        if (achievements == null) return;

        foreach (var achievement in achievements)
        {
            achievement.UpdateCompletion(this);
        }
    }

    // ===================
    // Public triggers
    // ===================
    public void OnServeLomi()
    {
        lomiServed++;
    }

    public bool AchievementUnlocked(string achievementName)
    {
        if (achievements == null) return false;

        Achievement[] achievementArray = achievements.ToArray();
        Achievement a = Array.Find(achievementArray, ach => achievementName == ach.title);

        return a != null && a.achieved;
    }

    // ===================
    // Popup handling
    // ===================
    public void ShowPopup(string title)
    {
        if (popupPrefab == null || popupParent == null)
        {
            Debug.LogWarning("Popup prefab or parent not assigned.");
            return;
        }

        GameObject popup = Instantiate(popupPrefab, popupParent);
        popup.transform.localPosition = Vector3.zero;

        // Update text
        TextMeshProUGUI text = popup.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
            text.text = $"Unlocked!\n{title}";

        // destroy after few seconds
        Destroy(popup, popupDuration);
    }
}

[System.Serializable]
public class Achievement
{
    public string title;
    public string description;
    public Predicate<object> requirement;
    public bool achieved;

    public Achievement(string title, string description, Predicate<object> requirement)
    {
        this.title = title;
        this.description = description;
        this.requirement = requirement;
        this.achieved = false;
    }

    public void UpdateCompletion(AchievementManager manager)
    {
        if (achieved) return;

        if (RequirementsMet())
        {
            achieved = true;
            Debug.Log($"Achievement Unlocked: {title}");
            manager.ShowPopup(title);
        }
    }

    private bool RequirementsMet()
    {
        return requirement.Invoke(null);
    }
}
