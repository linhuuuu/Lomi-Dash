using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ToppingContent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI toppingName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image image;

    public void InitContent(Topping topping)
    {
        toppingName.text = topping.toppingName;
        description.text = topping.description;
        image.sprite = topping.sprite;
        LayoutRebuilder.ForceRebuildLayoutImmediate(description.transform as RectTransform);
    }
}
