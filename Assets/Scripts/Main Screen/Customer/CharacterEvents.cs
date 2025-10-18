using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterEvent", menuName = "Drops/CharacterEvent")]
public class CharacterEvents : ScriptableObject
{
    public string id;
    public string characterName;
    public string eventName;
    public Sprite image;
}
