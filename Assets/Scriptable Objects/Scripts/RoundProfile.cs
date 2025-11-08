using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[CreateAssetMenu(fileName = "RoundProfile", menuName = "ScriptableObjects/RoundProfile")]

public class RoundProfile : ScriptableObject
{

    [Header("Meta")]
    public string roundName;

    public int level;
    public Locations location;
    public int difficulty;
    public float requiredFame;
    public float moneyQuota;

    [Header("StageUnlocks")]
    public List<CustomerData> customerUnlock;
    public Recipe recipeUnlock;
    public List<Recipe> availableRecipes;
    public List<Beverage> availableBeverages;
    public List<CustomerData> availableCustomers;
    public List<AlmanacEntryData> entryUnlocks;

    [Header("SpecialCustomer")]
    public CustomerData specialCustomerUnlock;
    public List<CustomerData> specialCustomers;

    [Header("Customer Group Generation Override")]
    public bool isGroupCountOverriden;
    [Range(1, 30)] public int minCustomerGroupCount, maxCustomerGroupCount;
    public float[] customerCountWeights = new float[5];
    public GameTheme theme;
}