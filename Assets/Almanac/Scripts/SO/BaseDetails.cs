using UnityEngine;
using System.Collections.Generic;

#region  BASE DATA CLASS
public abstract class AlmanacEntryData : ScriptableObject
{
    [Header(" General Info")]
    public string entryID;
    public string entryName;

    [TextArea(3, 10)]
    public string description;

    public Sprite mainImage;
}
#endregion

