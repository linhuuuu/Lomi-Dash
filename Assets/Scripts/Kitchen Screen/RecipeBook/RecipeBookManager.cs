using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeBookManager : MonoBehaviour
{
    //Location References
    [SerializeField] private Canvas recipeBookCanvas;
    [SerializeField] private Transform listContent;

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
        closeButton.onClick.AddListener(ToggleRecipeBook);
        recipeButton.onClick.AddListener(ToggleRecipeBook);
        if (GameManager.instance.state != GameManager.gameState.beforeDay)
            recipeButton.gameObject.SetActive(false);

        InitBaseContent();
        InitRecipeContent();
        // InitToppingContent();

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
            baseOptionCateg.InitTutOption($"{tut.id}", OpenBaseRecipeContent, tut);
        }

        OpenBaseRecipeContent(tutorials[0]);
    }

    private void InitRecipeContent()
    {
      
        recipes.InitCategory($"Recipes", ToggleRecList);

        //Options
        List<Recipe> ownedRecipeList = InventoryManager.inv.gameRepo.RecipeRepo;
        foreach (Recipe rec in ownedRecipeList)
        {
            RecipeBookOptions baseOptionCateg = GameObject.Instantiate(optionPrefab, recipeOptionsList);
            baseOptionCateg.InitRecOption($"{rec.id}", OpenRecipeContent, rec);
        }
    }

    private void InitToppingContent()
    {
        RecipeBookCateg toppingCateg = GameObject.Instantiate(categPrefab, listContent);
        toppingCateg.InitCategory($"Topping Content", ToggleToppingList);

        //Options
        List<Topping> ownedToppingList = InventoryManager.inv.playerRepo.ToppingRepo;
        foreach (Topping top in ownedToppingList)
        {
            RecipeBookOptions baseOptionCateg = GameObject.Instantiate(optionPrefab, listContent);
            baseOptionCateg.InitToppingOption($"{top.id}", OpenToppingContent, top);
        }
    }

    #endregion
    #region ToggleList

    private void ToggleToppingList() => toppingOptionsList.gameObject.SetActive(!toppingOptionsList.gameObject.activeSelf);

    private void ToggleRecList() => recipeOptionsList.gameObject.SetActive(!recipeOptionsList.gameObject.activeSelf);

    private void ToggleBaseList() => baseOptionsList.gameObject.SetActive(!baseOptionsList.gameObject.activeSelf);

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