using UnityEngine;
using Firebase.Firestore;
using System.Threading.Tasks;
using UnityEngine.SocialPlatforms.Impl;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class DataManager : MonoBehaviour
{
    private FirebaseFirestore db;
    public PlayerSaveData playerData { set; get; }

    public RoundResults results { set; get; }

    [field: SerializeField] public bool loaded { set; get; }
    [SerializeField] private bool isDebug = false;
    [SerializeField] private bool isNewTestUser = false;

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

    async void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        InitPlayerData();

        //Skipped Auth, load test data;
        if (SignInManager.instance == null || isNewTestUser == true)
        {
            isDebug = true;
            if (isNewTestUser == true)
                await CreateNewUser("0");

            await InitDataManager();
        }
    }

    private void InitPlayerData()
    {
        //Sets Default PlayerData
        playerData = new PlayerSaveData
        {
            day = 1,
            money = 100,
            happiness = 0,
            unlockedBeverageIds = new List<string>() { "WATER" },
            unlockedRecipeIds = new List<string>() { "BATANGAS" },
            unlockedCustomerIds = new List<string>() { "MOTHER" },
            dialogueFlags = new Dictionary<string, bool>
                {
                    { "intro", false },

                    { "Lipa_Easy_Before", false },
                    { "Lipa_Easy_After", false },
                    { "Lipa_Med_Before", false },
                    { "Lipa_Med_After", false },
                    { "Lipa_Hard_Before", false },
                    { "Lipa_Hard_After", false },
                },
        };
    }

    public async Task InitDataManager()
    {
        if (isDebug == false)
            await FetchPlayerDataAsync(GameManager.instance.uid);
        else
            await FetchPlayerDataAsync("0");  //Test Data   

        ApplyLoadedData();
    }


    #region Data Handling
    private async Task FetchPlayerDataAsync(string uid)
    {
        try
        {   //Try Find Player
            DocumentReference docRef = db.Collection("players").Document(uid);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            //If User exists, set playerdata
            if (snapshot.Exists)
            {
                playerData = snapshot.ConvertTo<PlayerSaveData>();
                if (Debug.isDebugBuild) Debug.Log("Loaded: " + playerData.playerName);
                loaded = true;
            }
            else
                await CreateNewUser(uid);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading Firestore data: " + e.Message);
        }
    }

    private async Task CreateNewUser(string uid)
    {
        playerData.userId = uid;
        await db.Collection("players").Document(uid).SetAsync(playerData);
        loaded = true;
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

    public async Task UpdatePlayerDataAsync(Dictionary<string, object> updates)
    {
        DocumentReference docRef = db.Collection("players").Document(playerData.userId);

        if (docRef == null)
        {
            if (Debug.isDebugBuild) Debug.Log("Player not found!");
            return;
        }

        try
        {
            await docRef.SetAsync(updates, SetOptions.MergeAll);
            ApplyUpdatesToLocalPlayerData(updates);
            if (Debug.isDebugBuild) Debug.Log("Player data updated");

        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save data: " + e.Message);
            // Retry Again Prompt
        }
    }

    private void ApplyUpdatesToLocalPlayerData(Dictionary<string, object> updates)
    {
        foreach (var entry in updates)
        {
            string key = entry.Key;
            object value = entry.Value;

            if (key == "dialogueFlags")
            {
                if (playerData.dialogueFlags == null)
                    playerData.dialogueFlags = new Dictionary<string, bool>();

                if (value is Dictionary<string, object> flags)
                {
                    foreach (var flag in flags.Keys)
                    {
                        if (flags[flag] is bool boolVal)
                            playerData.dialogueFlags[flag] = boolVal;
                    }
                }
            }

            if (key == "day" && value is int day) playerData.day = day;
            if (key == "money" && value is float money) playerData.money = money;
            if (key == "happiness" && value is float happiness) playerData.money = happiness;
        }
    }

    public async Task UploadRoundClearData(string level)
    {
        CollectionReference colRef = db.Collection($"{level}_Clears");

        if (colRef == null)
        {
            if (Debug.isDebugBuild) Debug.Log("Collection not found!");
            return;
        }

        try
        {
            await colRef.Document().SetAsync(results);
            if (Debug.isDebugBuild) Debug.Log(level + " Collection Updated");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to upload data: " + e.Message);
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
    //         if (Debug.IsDebugBuild) Debug.Log($"Player {newPlayerId} created from template.");
    //     }
    //     catch (System.Exception e)
    //     {
    //         Debug.LogError("Failed to create player from template: " + e.Message);
    //     }
    // }
    #endregion

}