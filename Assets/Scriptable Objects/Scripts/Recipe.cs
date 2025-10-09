using System.Collections.Generic;
using PCG;
using UnityEngine;


[CreateAssetMenu(fileName = "Recipe", menuName = "ScriptableObjects/Recipe")]
public class Recipe : ScriptableObject
{
    [System.Serializable]
    public class ToppingEntry
    {
        public Topping topping;
        [Min(1)] public int count = 1;
    }

    public string id;
    public string recipeName;
    public int basePrice;

    public List<ToppingEntry> toppingList = new List<ToppingEntry>();
    public GameObject toppingVisual;
};