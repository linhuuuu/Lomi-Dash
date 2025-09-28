using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Visual State Lib", menuName = "ScriptableObjects/VisualStateLib")]
public class VisualStateLib : ScriptableObject
{
    [System.Serializable]
    public class ColorDict
    {
        public string id;
        public Color value;
    }

    [System.Serializable]
    public class SpriteDict
    {
        public string id;
        public Sprite value;
    }

    [Header("Color")]
    public ColorDict[] broth, swirl;

    [Header("Sprite")]
    public SpriteDict[] bawang, onion;

    //Dictionaries
    public Dictionary<string, Color> brothColors = new Dictionary<string, Color>();
    public Dictionary<string, Color> swirlColors = new Dictionary<string, Color>();
    public Dictionary<string, Sprite> bawangStates = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> onionStates = new Dictionary<string, Sprite>();

    //Add Ditionaries
    void OnEnable()
    {
        InitDictionary(broth, brothColors);
        InitDictionary(swirl, swirlColors);
        InitDictionary(bawang, bawangStates);
    }

    //Helper Functions
    public void InitDictionary(ColorDict[] list, Dictionary<string, Color> dictionary)
    {
        foreach (var obj in list)
            dictionary.Add(obj.id, obj.value);
    }

    public void InitDictionary(SpriteDict[] list, Dictionary<string, Sprite> dictionary)
    {
        foreach (var obj in list)
            dictionary.Add(obj.id, obj.value);
    }

}