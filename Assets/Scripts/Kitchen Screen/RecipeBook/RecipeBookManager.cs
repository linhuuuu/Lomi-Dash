using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeBookManager : MonoBehaviour
{
    //Location References
    [SerializeField] private Canvas recipeBookCanvas;
    [SerializeField] private Transform listContent;
    [SerializeField] private Transform listOptionsContainer;

    //Content References
    [SerializeField] private BaseContent baseContent;
    [SerializeField] private ToppingContent toppingContent;
    [SerializeField] private RecipeContent recipeContent;

    //Prefab References
    [SerializeField] private RecipeBookCateg categPrefab;
    [SerializeField] private RecipeBookOptions optionPrefab;

    //List References
    [SerializeField] private RecipeBookCateg baseRecipe;
    [SerializeField] private RecipeBookCateg topping;
    [SerializeField] private RecipeBookCateg recipes;
    [SerializeField] private Transform recipeOptionsList;
    [SerializeField] private Transform toppingOptionsList;
    [SerializeField] private Transform baseOptionsList;

    //Buttons
    [SerializeField] private Button recipeButton;
    [SerializeField] private Button closeButton;

    void Awake()
    {
        baseContent.gameObject.SetActive(true);
        recipeContent.gameObject.SetActive(false);
        toppingContent.gameObject.SetActive(false);
    }

    void Start()
    {
        recipeBookCanvas.gameObject.SetActive(false);
        
        closeButton.onClick.AddListener(ToggleRecipeBook);
        recipeButton.onClick.AddListener(ToggleRecipeBook);
        if (GameManager.instance.state == GameManager.gameState.tutorial)
            recipeButton.gameObject.SetActive(false);

        InitBaseContent();
        InitRecipeContent();
        InitToppingContent();

        //Hide List
        ToggleBaseList();
        ToggleRecList();
        ToggleToppingList();
    }

    private void ToggleRecipeBook() => recipeBookCanvas.gameObject.SetActive(!recipeBookCanvas.gameObject.activeSelf);

    #region Contents
    private void InitBaseContent()
    {
        baseRecipe.InitCategory($"Lomi Base", ToggleBaseList);

        //Options
        List<Tutorial> tutorials = InventoryManager.inv.gameRepo.tutorials;
        foreach (var tut in tutorials)
        {
            RecipeBookOptions baseOptionCateg = GameObject.Instantiate(optionPrefab, baseOptionsList);
            baseOptionCateg.InitTutOption($"{tut.fieldName}", OpenBaseRecipeContent, tut);
        }

        OpenBaseRecipeContent(tutorials[0]);
        LayoutRebuilder.ForceRebuildLayoutImmediate(baseOptionsList.transform as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(listContent.transform as RectTransform);
    }

    private void InitRecipeContent()
    {
        recipes.InitCategory($"Recipes", ToggleRecList);

        //Options
        List<Recipe> ownedRecipeList = InventoryManager.inv.gameRepo.RecipeRepo;
        foreach (Recipe rec in ownedRecipeList)
        {
            RecipeBookOptions recOptionCateg = GameObject.Instantiate(optionPrefab, recipeOptionsList);
            recOptionCateg.InitRecOption($"{rec.id}", OpenRecipeContent, rec);

            if (!InventoryManager.inv.playerRepo.RecipeRepo.Contains(rec))
                recOptionCateg.DisableButton();

            LayoutRebuilder.ForceRebuildLayoutImmediate(recipeOptionsList.transform as RectTransform);
        }
    }

    private void InitToppingContent()
    {
        topping.InitCategory($"Topping", ToggleToppingList);

        //Options
        List<Topping> ownedToppingList = InventoryManager.inv.gameRepo.ToppingRepo;
        foreach (Topping top in ownedToppingList)
        {
            RecipeBookOptions toppingOption = GameObject.Instantiate(optionPrefab, toppingOptionsList);
            toppingOption.InitToppingOption($"{top.toppingName}", OpenToppingContent, top);

            if (!InventoryManager.inv.playerRepo.ToppingRepo.Contains(top))
                toppingOption.DisableButton();
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(toppingOptionsList.transform as RectTransform);
    }

    #endregion
    #region ToggleList


    private void ToggleBaseList()
    {
        baseOptionsList.gameObject.SetActive(!baseOptionsList.gameObject.activeSelf);
        LayoutRebuilder.ForceRebuildLayoutImmediate(baseOptionsList.transform as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(listContent.transform as RectTransform);
    }

    private void ToggleRecList()
    {
        recipeOptionsList.gameObject.SetActive(!recipeOptionsList.gameObject.activeSelf);
        LayoutRebuilder.ForceRebuildLayoutImmediate(recipeOptionsList.transform as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(listContent.transform as RectTransform);
    }

    private void ToggleToppingList()
    {
        toppingOptionsList.gameObject.SetActive(!toppingOptionsList.gameObject.activeSelf);
        LayoutRebuilder.ForceRebuildLayoutImmediate(toppingOptionsList.transform as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(listContent.transform as RectTransform);
    }

    #endregion
    #region OpenContent
    private void OpenBaseRecipeContent(Tutorial tutorial)
    {
        baseContent.gameObject.SetActive(true);
        recipeContent.gameObject.SetActive(false);
        toppingContent.gameObject.SetActive(false);

        baseContent.InitContent(tutorial);

    }

    private void OpenRecipeContent(Recipe rec)
    {
        baseContent.gameObject.SetActive(false);
        recipeContent.gameObject.SetActive(true);
        toppingContent.gameObject.SetActive(false);

        recipeContent.InitContent(rec);
    }

    private void OpenToppingContent(Topping top)
    {
        baseContent.gameObject.SetActive(false);
        recipeContent.gameObject.SetActive(false);
        toppingContent.gameObject.SetActive(true);

        toppingContent.InitContent(top);
    }
    #endregion
}