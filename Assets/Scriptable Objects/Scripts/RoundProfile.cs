using UnityEngine;

[CreateAssetMenu(fileName = "RoundProfile", menuName = "ScriptableObjects/RoundProfile")]

public class RoundProfile : ScriptableObject
{

    [Header("Meta")]
    public string roundName;
    public int level;
    public Locations location;
    public int difficulty;

    [Header("Customer Group Generation Override")]
    public bool isGroupCountOverriden;
    [Range(1,30)] public int minCustomerGroupCount, maxCustomerGroupCount;
    public float[] customerCountWeights = new float[5];
    public GameTheme theme;
}