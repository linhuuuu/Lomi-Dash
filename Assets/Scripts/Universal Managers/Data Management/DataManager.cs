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

    public static DataManager data;
    void Awake()
    {
        if (data == null)
        {
            data = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        playerData = new PlayerSaveData();
        playerData.largeBowlUnlocked = true;
        playerData.largeTrayUnlocked = true;
    }
    

    #region Data Handling

    public void Start()
    {

        // await LoadPlayerData();
        // ApplyLoadedData();
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
            Beverage bev = InventoryManager.inv.gameRepo.BeverageRepo.Find(b => b.id == bevId);
            if (bev != null)
                InventoryManager.inv.playerRepo.BeverageRepo.Add(bev);
        }

        foreach (string recipeId in playerData.unlockedRecipeIds)
        {
            Recipe rec = InventoryManager.inv.gameRepo.RecipeRepo.Find(b => b.id == recipeId);
            if (rec != null)
                InventoryManager.inv.playerRepo.RecipeRepo.Add(rec);
        }

        foreach (string customerId in playerData.unlockedCustomerIds)
        {
            CustomerData cus = InventoryManager.inv.gameRepo.CustomerRepo.Find(b => b.id == customerId);
            if (cus != null)
                InventoryManager.inv.playerRepo.CustomerRepo.Add(cus);
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