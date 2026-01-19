using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Notification : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private TextMeshProUGUI desc;

    public void InitNotification(string type, string name)
    {
        if (type == "location" || type == "term" || type == "customer")
        {
            image.sprite = sprites[0];
            header.text = "Updated Almanac!";

            if (type == "location")
            {
                desc.text = $"Unlocked new location: {name}";
            }

            if (type == "term")
            {
                desc.text = $"Unlocked new term: {name}";
            }

            if (type == "character")
            {
                desc.text = $"Updated Character Entries: {name}";
            }
        }
        else if (type == "recipe")
        {
            image.sprite = sprites[1];
            header.text = "Updated RecipeBook!";

            desc.text = $"Unlocked new recipe: {name}";
        }

        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
    }
}