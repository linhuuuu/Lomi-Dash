using UnityEngine;
[System.Serializable]
    public class PlayerData
{
    [Header("Info")]
    public string playerId;

    [Header("Kitchen")]
    public bool largeBowlUnlocked;
    public bool largeTrayUnlocked;

    [Header("Economy")]

    public int happiness;
    public int money;
}