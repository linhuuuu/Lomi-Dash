using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VisualStateLib", menuName = "ScriptableObjects/VisualStateLib")]
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
    public ColorDict[] brothcolor, noodlescolor;

    [Header("Sprite")]
    public SpriteDict[] bawang, oil, onion, broth, egg, thickener, seasoningTray, sinkWater, sink, potSeasoning, water;


    //Dictionaries

    //Pot
    public Dictionary<string, Sprite> sinkWaterStates = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> sinkStates = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> waterStates = new Dictionary<string, Sprite>();


    public Dictionary<string, Color> brothColors = new Dictionary<string, Color>();
    public Dictionary<string, Color> noodlesColors = new Dictionary<string, Color>();

    public Dictionary<string, Sprite> bawangStates = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> onionStates = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> oilStates = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> eggStates = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> thickenerStates = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> brothStates = new Dictionary<string, Sprite>();

    public Dictionary<string, Sprite> seasoningTrayStates = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> potSeasoningStates = new Dictionary<string, Sprite>();


    //Add Ditionaries
    void OnEnable()
    {
        InitDictionary(brothcolor, brothColors);
        InitDictionary(bawang, bawangStates);
        InitDictionary(onion, onionStates);
        InitDictionary(oil, oilStates);
        InitDictionary(egg, eggStates);
        InitDictionary(thickener, thickenerStates);
        InitDictionary(broth, brothStates);

        InitDictionary(noodlescolor, noodlesColors);
        InitDictionary(seasoningTray, seasoningTrayStates);
        InitDictionary(sinkWater, sinkWaterStates);
        InitDictionary(sink, sinkStates);
        InitDictionary(water, waterStates);

        InitDictionary(potSeasoning, potSeasoningStates);
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