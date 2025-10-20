using System.Collections.Generic;
using UnityEngine;
#region CUSTOMER DATA
[CreateAssetMenu(menuName = "Almanac/Customer Data")]
public class SpecialNPCData: AlmanacEntryData
{
    [Header("Customer-Specific Fields")]
    [Range(0, 3)] public int starCount = 0;
    public List<string> characterEvents;

    public string customerLocation;
}
#endregion