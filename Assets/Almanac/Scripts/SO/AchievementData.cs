using UnityEngine;

#region ACHIEVEMENT DATA
[CreateAssetMenu(menuName = "Almanac/Achievement Data")]
public class AchievementData : AlmanacEntryData
{
    public string reward;
    [Header("Achievement-Specific Fields")]

    public bool unlockedByDefault;
}

#endregion