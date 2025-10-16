using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager inv;

    public VisualState globalVisualState;

    void Awake()
    {
        if (inv == null)
        {
            inv = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    //Repositories
    [System.Serializable]
    public class Repositories
    {
        public List<Recipe> RecipeRepo;
        public List<Beverage> BeverageRepo;
        public List<CustomerData> CustomerRepo;
        public List<Topping> ToppingRepo;
        public List<Drop> DropsRepo;
        // public List<Achievement> AchievementRepo;
    }

    public Repositories gameRepo;
    public Repositories playerRepo;

    // public async Task InitializeAsync()
    // {
        
    // }
}
