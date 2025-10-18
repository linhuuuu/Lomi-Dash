using UnityEngine;
using Firebase.Firestore;
using System.Threading.Tasks;
using UnityEngine.SocialPlatforms.Impl;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using System.Net;
using Firebase;
using Firebase.Auth;

public class DataManager : MonoBehaviour
{
    private FirebaseFirestore db;
    public PlayerSaveData playerData { set; get; }

    public RoundResults results { set; get; }

    [field: SerializeField] public bool loaded { set; get; }
    [SerializeField] private bool isDebug = false;
    [SerializeField] private bool isNewTestUser = false;

    FirebaseAuth auth;

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
            auth = FirebaseAuth.DefaultInstance;
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
            unlockedLocationIds = new List<string>() { "LIPA" },
            unlockedTermIds = new List<string>() { "LOMI" },
            unlockedSpecialCustomerIds = new List<string>() { "JUAN" },
            unlockedAchievementIds = new List<string>() { },

            //UPDATE IN DATABASE
            characterEvents = new Dictionary<string, List<bool>>()
            {
                {"JUAN", new List<bool> {false, false, false}},
                {"TIYA_XIAO", new List<bool> {false, false, false}},
                {"FATHER", new List<bool> {false, false, false}},
            }
        };
    }

    public async Task InitDataManager()
    {
        if (isDebug == false)
            await FetchPlayerDataAsync(GameManager.instance.uid);
        else
            await FetchPlayerDataAsync(auth.CurrentUser.UserId);  //Test Data   

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

        //Almanac
        foreach (string locationId in playerData.unlockedLocationIds)
        {
            LocationData cus = InventoryManager.inv.gameRepo.LocationRepo.Find(b => b.entryID == locationId);
            if (cus != null)
                InventoryManager.inv.playerRepo.LocationRepo.Add(cus);
        }

        foreach (string termId in playerData.unlockedTermIds)
        {
            TermData term = InventoryManager.inv.gameRepo.TermRepo.Find(b => b.entryID == termId);
            if (term != null)
                InventoryManager.inv.playerRepo.TermRepo.Add(term);
        }

        foreach (string specialNPCId in playerData.unlockedSpecialCustomerIds)
        {
            SpecialNPCData npc = InventoryManager.inv.gameRepo.SpecialNPCRepo.Find(b => b.entryID == specialNPCId);
            if (npc != null)
                InventoryManager.inv.playerRepo.SpecialNPCRepo.Add(npc);
        }

        foreach (string achId in playerData.unlockedAchievementIds)
        {
            AchievementData ach = InventoryManager.inv.gameRepo.AchievementRepo.Find(b => b.entryID == achId);
            if (ach != null)
                InventoryManager.inv.playerRepo.AchievementRepo.Add(ach);
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

    #region Queries

    public class PlayerLeaderBoardData
    {
        public string userId;
        public string name;
        public int icon;
    }
    public class MoneyLeaderBoardData
    {
        public PlayerLeaderBoardData ply;
        public float money;
    }

    public class HappinessLeaderBoardData
    {
        public PlayerLeaderBoardData ply;
        public float happiness;
    }

    public List<MoneyLeaderBoardData> moneylbData { set; get; } = new();
    public List<HappinessLeaderBoardData> happinesslbData { set; get; } = new();

    [SerializeField] private int queryCount = 5;

    public async Task FetchLeaderBoardData(string collectionRefName)
    {
        CollectionReference colRef = db.Collection(collectionRefName + "_Clears");

        QuerySnapshot moneyQuerySnap = await colRef.OrderByDescending("earnedMoney").Limit(queryCount).GetSnapshotAsync();
        foreach (DocumentSnapshot snap in moneyQuerySnap)
        {
            string uid = snap.GetValue<string>("userId");
            float mny = snap.GetValue<float>("earnedMoney");

            if (mny == 0)
                continue;

            //Check Duplicates
            MoneyLeaderBoardData duplicate = moneylbData.Find(c => c.ply.userId == uid);
            if (duplicate != null)
            {
                if (mny > duplicate.money)
                    moneylbData.Remove(duplicate);
                else
                    continue;
            }

            DocumentSnapshot snapPlayer = await db.Collection("players").Document(uid).GetSnapshotAsync();
            if (snapPlayer == null)
            {
                if (Debug.isDebugBuild) Debug.Log("Player not found!");
                return;
            }

            try
            {
                PlayerSaveData playerdata = snapPlayer.ConvertTo<PlayerSaveData>();
                moneylbData.Add(new MoneyLeaderBoardData
                {
                    ply = new PlayerLeaderBoardData
                    {
                        userId = uid,
                        name = playerdata.playerName,
                        icon = playerdata.icon,

                    },
                    money = mny,
                });
            }

            catch (System.Exception e)
            {
                Debug.LogError("Failed to fetch data: " + e.Message);
            }
        }

        QuerySnapshot happinessQuerySnap = await colRef.OrderByDescending("earnedHappiness").Limit(queryCount).GetSnapshotAsync();
        foreach (DocumentSnapshot snap in happinessQuerySnap)
        {
            string uid = snap.GetValue<string>("userId");
            float hp = snap.GetValue<float>("earnedHappiness");

            //Check Duplicates
            HappinessLeaderBoardData duplicate = happinesslbData.Find(c => c.ply.userId == uid);
            if (duplicate != null)
            {
                if (hp > duplicate.happiness)
                    happinesslbData.Remove(duplicate);
                else
                    continue;
            }

            DocumentSnapshot snapPlayer = await db.Collection("players").Document(uid).GetSnapshotAsync();
            if (snapPlayer == null)
            {
                if (Debug.isDebugBuild) Debug.Log("Player not found!");
                return;
            }

            try
            {
                PlayerSaveData playerdata = snapPlayer.ConvertTo<PlayerSaveData>();
                happinesslbData.Add(new HappinessLeaderBoardData
                {
                    ply = new PlayerLeaderBoardData
                    {
                        userId = uid,
                        name = playerdata.playerName,
                        icon = playerdata.icon,
                    },
                    happiness = hp,
                });
            }

            catch (System.Exception e)
            {
                Debug.LogError("Failed to fetch data: " + e.Message);
            }
        }
    }

    #endregion

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