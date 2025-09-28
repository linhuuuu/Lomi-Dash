using UnityEngine;
using Firebase.Firestore;
using System.Threading.Tasks;
using UnityEngine.SocialPlatforms.Impl;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public class LatestRoundResults
    {
        int dishesCleared, happyCustomers, unhappyCustomers;
        float earnedHappiness, earnedMoney;
    }

    public LatestRoundResults results;
    public PlayerSaveData playerData;

    //Repositories
    [System.Serializable]
    public class Repositories
    {

        public List<Recipe> RecipeRepo;
        public List<Beverage> BeverageRepo;
        public List<CustomerData> CustomerRepo;
        public List<Achievement> AchievementRepo;
    }

    public Repositories playerRepo;
    public Repositories gameRepo;
    
    
    public static DataManager data;
    void Awake()
    {
        if (data == null)
        {
            data = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    #region Data Handling

    async void Start()
    {
        await LoadPlayerData();
        ApplyLoadedData();
    }

    private async Task LoadPlayerData()
    {
        try
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            string userId = "0"; // Replace with actual user ID from Auth

            DocumentReference docRef = db.Collection("players").Document(userId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                PlayerSaveData savedData = snapshot.ConvertTo<PlayerSaveData>();
                Debug.Log("Loaded: " + savedData.playerName);
            }
            
            else
            {
                // Create new player with defaults
                playerData = new PlayerSaveData
                {
                    userId = userId,
                    money = 100,
                    happiness = 0,
                    unlockedBeverageIds = new List<string>() { "WATER" }, // default unlock
                    unlockedRecipeIds = new List<string>() { "MOTHER" },
                    unlockedCustomerIds = new List<string>() { "BATANGAS" }
                };

                await db.Collection("players").Document(userId).SetAsync(playerData);
            }
        } 
        catch (System.Exception e)
        {
            Debug.LogError("Error loading Firestore data: " + e.Message);
        }
    }

    void ApplyLoadedData()
    {
        foreach (string bevId in playerData.unlockedBeverageIds)
        {
            Beverage bev = gameRepo.BeverageRepo.Find(b => b.id == bevId);
            if (bev != null)
                playerRepo.BeverageRepo.Add(bev);
        }

        foreach (string recipeId in playerData.unlockedRecipeIds)
        {
            Recipe rec = gameRepo.RecipeRepo.Find(b => b.id == recipeId);
            if (rec != null)
                playerRepo.RecipeRepo.Add(rec);
        }

        foreach (string customerId in playerData.unlockedCustomerIds)
        {
            CustomerData cus = gameRepo.CustomerRepo.Find(b => b.id == customerId);
            if (cus != null)
                playerRepo.CustomerRepo.Add(cus);
        }
    }

    public async void SavePlayerData()
    {
        var db = FirebaseFirestore.DefaultInstance;
        string userId = "0"; //FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        await db.Collection("players").Document(userId).SetAsync(playerData);
    }

    #endregion


}