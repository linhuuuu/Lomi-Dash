using TMPro;
using UnityEngine;

public class RecipeContent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeName;
    [SerializeField] private TextMeshProUGUI description;
    
    [SerializeField] private GameObject dish;
    [SerializeField] private ToppingDetail toppingDetail;
    [SerializeField] private Transform toppingDetailTransform;

    public void InitContent(Recipe rec)
    {
        //Header
        recipeName.text = $"{rec.recipeName}";
        description.text = $"{rec.description}";

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