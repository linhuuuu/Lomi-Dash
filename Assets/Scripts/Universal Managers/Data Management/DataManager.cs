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

    private FirebaseFirestore db;
    public PlayerSaveData playerData;
    public LatestRoundResults results;

    [field : SerializeField]public bool loaded { set; get; }
    [SerializeField] private bool isDebug = false;

    public static DataManager data;

    void Awake()
    {
        if (data == null)
        {
            data = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        InitPlayerData();
        db = FirebaseFirestore.DefaultInstance;

        if (SignInManager.instance == null)
            isDebug = true;
    }

    private void InitPlayerData()
    {
        playerData = new PlayerSaveData();

        playerData.dialogueFlags = new Dictionary<string, bool>
        {
            { "intro", true },

        };
    }

    public async Task InitDataManager()
    {
        if (isDebug == true)
            await LoadPlayerData("0");  //Test User   
        else
            await LoadPlayerData(GameManager.instance.uid);      
    }

    #region Data Handling

    private async Task LoadPlayerData(string uid)
    {
        try
        {
            db = FirebaseFirestore.DefaultInstance;
            string userId = uid;

            DocumentReference docRef = db.Collection("players").Document(userId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                playerData = snapshot.ConvertTo<PlayerSaveData>();
                Debug.Log("Loaded: " + playerData.playerName);
                loaded = true;
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
        string userId = "0"; //FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        await db.Collection("players").Document(userId).SetAsync(playerData);
    }

    public async Task UpdatePlayerDataAsync(Dictionary<string, object> updates)
    {
                Debug.Log(playerData.userId);
        Debug.Log(updates);

        DocumentReference docRef = db.Collection("players").Document(playerData.userId);

        if (docRef == null)
        {
            Debug.Log("✅ Player not found!");
            return;
        }

        try
        {
            await docRef.SetAsync(updates, SetOptions.MergeAll);
            Debug.Log("✅ Player data updated");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save data: " + e.Message);
            // Retry Again Prompt
        }
    }

    // public async Task CreatePlayerFromTemplate(string newPlayerId)
    // {
    //     DocumentReference templateRef = db.Collection("players").Document("player_template");
    //     DocumentReference newPlayerRef = db.Collection("players").Document(newPlayerId);

    //     try
    //     {
    //         // 1. Load the template document
    //         DocumentSnapshot snapshot = await templateRef.GetSnapshotAsync();

    //         if (!snapshot.Exists)
    //         {
    //             Debug.LogError("Template document 'player_template' not found!");
    //             return;
    //         }

    //         // 2. Get all data (including nested maps like flags, tools, etc.)
    //         Dictionary<string, object> templateData = snapshot.ToDictionary();

    //         // 3. Optionally modify values (e.g., set displayName, timestamp)
    //         if (templateData.ContainsKey("profile") && 
    //             templateData["profile"] is Dictionary<string, object> profile)
    //         {
    //             // Example: Set default name or personalize
    //             profile["displayName"] = "a"; 
    //         }

    //         // // Update metadata
    //         // templateData["createdAt"] = FieldValue.ServerTimestamp();
    //         // templateData["lastLoginAt"] = FieldValue.ServerTimestamp();

    //         // 4. Create new player using template
    //         await newPlayerRef.SetAsync(templateData);
    //         Debug.Log($"Player {newPlayerId} created from template.");
    //     }
    //     catch (System.Exception e)
    //     {
    //         Debug.LogError("Failed to create player from template: " + e.Message);
    //     }
    // }

    #endregion

}