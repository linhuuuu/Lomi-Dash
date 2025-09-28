using UnityEngine;


[CreateAssetMenu(fileName = "Topping", menuName = "ScriptableObjects/Topping")]
public class Topping : ScriptableObject
{
        public int id;
        public string toppingName;
        public Sprite sprite;
};