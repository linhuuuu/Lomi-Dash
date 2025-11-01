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

    [Header("StageUnlocks")]
    public List<CustomerData> customerUnlock;
    public Recipe recipeUnlock;
    public Beverage beverageUnlock;
    public List<Recipe> availableRecipes;
    public List<AlmanacEntryData> entryUnlocks;


    [Header("SpecialCustomer")]
    [Max(4)] public List<CustomerData> companions;
    public CustomerData specialCustomerUnlock;
    public List<CustomerData> specialCustomers;

    [Header("Customer Group Generation Override")]
    public bool isGroupCountOverriden;
    [Range(1, 30)] public int minCustomerGroupCount, maxCustomerGroupCount;
    public float[] customerCountWeights = new float[5];
    public GameTheme theme;
}