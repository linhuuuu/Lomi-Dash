using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterEvent", menuName = "ScriptableObjects/CharacterEvent")]
public class CharacterEvent : ScriptableObject
{
    public string id;
    public List<AlmanacEntryData> unlockEntryData;
    public List<BuffData> unlockBuffs;
}