using UnityEngine;


[CreateAssetMenu(fileName = "Topping", menuName = "ScriptableObjects/Topping")]
public class Topping : ScriptableObject
{
        public enum toppingType
        {
                single,
                garnish,
        }

        public string id;
        public string toppingName;
        public Sprite sprite;
        public Sprite containerSprite;
        public Sprite garnishSprite;
        public toppingType type;
};