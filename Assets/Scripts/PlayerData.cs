using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="PlayerData", menuName ="ScriptableObjects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Info")]
    public string playerId;

    [Header("Kitchen")]
    public bool largeBowlUnlocked;
    public bool largeTrayUnlocked;

    [Header("Economy")]

    public int happiness;
    public int money;

    [Header("Unlocked")]
    public List<Beverage> beverages;
    public List<Recipe> recipes;
    public List<CustomerData> customerList;
}