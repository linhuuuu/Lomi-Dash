using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class RecipeBookOptions : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fieldName;
    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    [SerializeField] private Recipe recipe;
    [SerializeField] private Topping topping;
     [SerializeField] private Tutorial tutorial;


    public void InitRecOption(string name, Action<Recipe> action, Recipe recipe)
    {
        fieldName.text = name;
        button.onClick.AddListener(() => action(recipe));

        transform.localScale = Vector3.one;
        transform.localEulerAngles = Vector3.one;

        this.recipe = recipe;
    }

     public void InitToppingOption(string name, Action<Topping> action, Topping topping)
    {
        fieldName.text = name;
        button.onClick.AddListener(() => action(topping));

        transform.localScale = Vector3.one;
        transform.localEulerAngles = Vector3.one;

        this.topping = topping;
    }

     public void InitTutOption(string name, Action<Tutorial> action, Tutorial tutorial)
    {
        fieldName.text = name;
        button.onClick.AddListener(() => action(tutorial));

        transform.localScale = Vector3.one;
        transform.localEulerAngles = Vector3.one;

        this.tutorial = tutorial;
    }
}