using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Almanac/Location Data")]
public class LocationData : AlmanacEntryData
{
    [Header("Location-Specific Fields")]
    public List<Sprite> lomiImages = new List<Sprite>();
    
    [TextArea(2, 5)]
    public string trivia;
}

