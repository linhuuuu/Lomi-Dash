using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RecipeContent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeName;
    [SerializeField] private TextMeshProUGUI description;

    [SerializeField] private GameObject dish;
    [SerializeField] private ToppingDetail toppingDetail;
    [SerializeField] private Transform toppingDetailTransform;
    [SerializeField] private ToppingDetail[] toppingdetails;

    //Recipe Visual
    private Dictionary<string, GameObject> recipe = new();
    private GameObject currentRecipeVisual;

    public void InitContent(Recipe rec)
    {
        //Header
        recipeName.text = $"{rec.recipeName}";
        description.text = $"{rec.description}";

        //OldToppingVisual
        if (recipe.ContainsKey(rec.id))
        {
            if (currentRecipeVisual != recipe[rec.id])  //Switch to existing
            {
                //disable current
                currentRecipeVisual.SetActive(false);

                recipe[rec.id].SetActive(true);
                currentRecipeVisual = recipe[rec.id];
            }
        }
        else
        {
            //New Topping Visual - Add to Dictionary
            GameObject toppingGroup = Instantiate(rec.toppingVisual, Vector3.zero, Quaternion.identity, dish.transform);
            toppingGroup.transform.SetSiblingIndex(1);
            toppingGroup.transform.localEulerAngles = Vector3.zero;
            toppingGroup.transform.localPosition = Vector3.zero;
            toppingGroup.transform.localScale = new Vector3(100f, 100f, 100f);


            currentRecipeVisual?.gameObject.SetActive(false);
            currentRecipeVisual = toppingGroup;
            recipe.Add(rec.id, toppingGroup);
        }

        //Topping List
List<Recipe.ToppingEntry> toppings = rec.toppingList;
for (int j = 0; j < toppingdetails.Length; j++)
{
    if (j < toppings.Count)
    {
        toppingdetails[j].gameObject.SetActive(true);
        toppingdetails[j].InitToppingDetail(toppings[j].topping.sprite, toppings[j].count);
    }
    else
    {
        // Optional: disable unused topping detail UI elements
        toppingdetails[j].gameObject.SetActive(false);
    }
}
    }
}