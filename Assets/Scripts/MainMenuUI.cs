using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    
    public void OnClickRecipeBook()
    {
        Debug.Log("Recipe Book clicked!");
        //SceneManager.LoadScene("RecipeScene"); 
    }

    public void OnClickKitchen()
    {
        Debug.Log("Kitchen clicked!");
        //SceneManager.LoadScene("KitchenScene");
    }

    public void OnClickShop()
    {
        Debug.Log("Shop clicked!");
        //SceneManager.LoadScene("ShopScene");
    }

    public void OnClickSettings()
    {
        Debug.Log("Settings clicked!");
        // Open a panel or settings menu here
    }
}
