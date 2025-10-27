using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Dialogue/CharacterDatabase")]
public class CharacterDatabase : ScriptableObject
{
    [System.Serializable]
    public class CharacterData
    {
        public string name;
        public Sprite defaultSprite;
        public List<CharacterEmotion> sprites;
        public Sprite chibiPortrait;

    }
    
    [System.Serializable]
    public class CharacterEmotion
    {
        public string id;
        public Sprite sprite;
    }

  
    public List<CharacterData> characters = new List<CharacterData>();

    public CharacterData GetCharacter(string name)
    {
        return characters.Find(c => c.name == name);
    }
}
