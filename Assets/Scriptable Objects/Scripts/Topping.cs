using UnityEngine;


[CreateAssetMenu(fileName = "Topping", menuName = "ScriptableObjects/Topping")]
public class Topping : ScriptableObject
{
        public string id;
        public string toppingName;
        public Sprite sprite;
};