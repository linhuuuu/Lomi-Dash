using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class RecipeBookOptions : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fieldName;
    [SerializeField] private Image icon;
    [SerializeField] public Button button;
    [SerializeField] private Recipe recipe;
    [SerializeField] private Topping topping;
    [SerializeField] private Tutorial tutorial;


    public void InitRecOption(string name, Action<Recipe> action, Recipe recipe)
    {
        fieldName.text = name;
        button.onClick.AddListener(() => action(recipe));
        icon.gameObject.SetActive(false);   //change

        transform.localScale = Vector3.one;
        transform.localEulerAngles = Vector3.one;

        this.recipe = recipe;
    }

    public void InitToppingOption(string name, Action<Topping> action, Topping topping)
    {
        fieldName.text = name;
        button.onClick.AddListener(() => action(topping));
        icon.sprite = topping.sprite;

        transform.localScale = Vector3.one;
        transform.localEulerAngles = Vector3.one;

        this.topping = topping;
        this.transform.SetAsFirstSibling();
    }

    public void InitTutOption(string name, Action<Tutorial> action, Tutorial tutorial)
    {
        fieldName.text = name;
        button.onClick.AddListener(() => action(tutorial));
        icon.gameObject.SetActive(false);

        transform.localScale = Vector3.one;
        transform.localEulerAngles = Vector3.one;

        this.tutorial = tutorial;
    }

    public void DisableButton()
    {
        button.interactable = false;
        fieldName.text = "PROGRESS TO UNLOCK!";
        icon.gameObject.SetActive(false);
    }
}