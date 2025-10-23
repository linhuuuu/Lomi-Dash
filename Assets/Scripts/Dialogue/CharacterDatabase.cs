using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Dialogue/CharacterDatabase")]
public class CharacterDatabase : ScriptableObject
{
    [System.Serializable]
    public class CharacterData
    {
        public string name;
        public Sprite portrait;
        public Vector2 anchoredPosition;
        public Sprite chibiPortrait;

    }

    public List<CharacterData> characters = new List<CharacterData>();

    public CharacterData GetCharacter(string name)
    {
        return characters.Find(c => c.name == name);
    }
}
