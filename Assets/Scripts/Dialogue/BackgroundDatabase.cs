using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BackgroundDatabase", menuName = "Dialogue/BackgroundDatabase")]
public class BackgroundDatabase : ScriptableObject
{
    [System.Serializable]
    public class BGData
    {
        public string name;
        public Sprite image;

    }

    public List<BGData> bgs = new();
}
