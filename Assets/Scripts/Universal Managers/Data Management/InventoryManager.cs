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
        public List<Recipe> RecipeRepo = new();
        public List<Beverage> BeverageRepo = new();
        public List<CustomerData> CustomerRepo = new();
        public List<Topping> ToppingRepo = new();
        public List<Drop> DropsRepo = new();
        public List<Sprite> Icons = new();
        public List<BuffData> BuffsRepo = new();
        public List<RoundProfile> roundProfiles = new();

      
        //ALAMANAC
        public List<LocationData> LocationRepo = new();
        public List<TermData> TermRepo = new();
        public List<SpecialNPCData> SpecialNPCRepo = new();
        public List<AchievementData> AchievementRepo = new();

        // //Inventory
        // public Dictionary<string, int> OwnedBuffs = new();
       
    }

    public Repositories gameRepo;
    public Repositories playerRepo;
}
