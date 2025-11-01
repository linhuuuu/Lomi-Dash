using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffData", menuName = "ScriptableObjects/Buffs")]
public class BuffData : ScriptableObject
{
    [System.Serializable]
    public enum BuffType
    {

        moneyBuff,
        happinessBuff,
        energyBuff,
        priceBuff,
        scoreBuff,
    }

     [System.Serializable]
    public class Buff
    {   
        public BuffType type;
        public float factor;
        public float subtrahend;
        public float addend;
    }

    public string id;
    public string buffName;
    [TextArea(3, 10)]public string effect;
    public Sprite sprite;
    public List<Buff> buffs;
}