using TMPro;
using UnityEngine;

public class DishSection : MonoBehaviour
{

    [SerializeField] private GameObject dish;
    [SerializeField] private ToppingDetail toppingDetail;
    [SerializeField] private Transform toppingDetailTransform;
    [SerializeField] private TextMeshProUGUI dishHeader;
    [SerializeField] private TextMeshProUGUI recipe;

    public void InitDishSection(Recipe rec, int i, bool isLarge)
    {
        //Header
        dishHeader.text = $"DISH {i}";
        string size = isLarge ? "Large" : "Regular";
        string recipeText = $"{rec.recipeName} ({size})";
        recipe.text = recipeText;

        //Topping Visual
        GameObject toppingGroup = Instantiate(rec.toppingVisual, Vector3.zero, Quaternion.identity, dish.transform);
        toppingGroup.transform.SetSiblingIndex(1);
        toppingGroup.transform.localEulerAngles = Vector3.zero;
        toppingGroup.transform.localPosition = Vector3.zero;
        toppingGroup.transform.localScale = new Vector3(100f, 100f, 100f);

        foreach (var topping in rec.toppingList)
        {
            ToppingDetail top = Instantiate(toppingDetail.gameObject, Vector3.one, Quaternion.identity, toppingDetailTransform).GetComponent<ToppingDetail>();
            top.InitToppingDetail(topping.topping.sprite, topping.count);
        }

        toppingDetail.gameObject.SetActive(false);
    }
}