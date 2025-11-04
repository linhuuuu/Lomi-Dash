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
using System.Linq;

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
            unlockedCustomerIds = new List<string>() { "MOTHER", "CHILD" },
            unlockedToppingIds = new List<string>() { "LIVER", "KIKIAM", "BOLA-BOLA", "EGG" },
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
            unlockedSpecialCustomerIds = new Dictionary<string, List<bool>>()
            {
                {"JUAN", new List<bool> {false, false, false}},
            },

            unlockedAchievementIds = new List<string>() { },

            unlockedKitchenTools = new Dictionary<string, int>
                {
                    { "Pot_1", 1 },
                    { "Pot_2", 0 },
                    { "Wok_1", 1 },
                    { "Wok_2", 0 },
                    { "DishRack", 1 },
                },
            unlockedBuffs = new Dictionary<string, int>
            {
                {"LIVER", 1},
                {"BOLA-BOLA", 1},
                {"EGG", 1},
                {"PORK_MEAT", 1},
                {"KIKIAM", 1}
            },
            clearStars = new Dictionary<string, int>
            {
                {"Lipa_Easy", 0},
                {"Lipa_Med", 0},
                {"Lipa_Hard", 0},
//improve this somehow

            },
            accountCreated = System.DateTime.Now,
            highestLevelCleared = 0,    //Start at Level 1
        };
    }

    public async Task InitDataManager()
    {
        if (isDebug == false)
            await FetchPlayerDataAsync(GameManager.instance.uid);
        else
            await FetchPlayerDataAsync(auth.CurrentUser.UserId);  //Test Data   

        ApplyLoadedData();
        await UpdatePlayerDataAsync(new Dictionary<string, object>
        {
            {"lastLogin", System.DateTime.Now}
        });
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
        InventoryManager.inv.playerRepo = new();

        foreach (string bevId in playerData.unlockedBeverageIds)
        {
            Beverage bev = InventoryManager.inv.gameRepo.BeverageRepo.Find(b => b.id == bevId);
            if (bev != null)
            {
                InventoryManager.inv.playerRepo.BeverageRepo.Add(bev);
            }

        }

        foreach (string topId in playerData.unlockedToppingIds)
        {
            Topping top = InventoryManager.inv.gameRepo.ToppingRepo.Find(b => b.id == topId);
            if (top != null)
            {
                InventoryManager.inv.playerRepo.ToppingRepo.Add(top);
            }

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

        foreach (var specialNPCId in playerData.unlockedSpecialCustomerIds)
        {
            SpecialNPCData npc = InventoryManager.inv.gameRepo.SpecialNPCRepo.Find(b => b.entryID == specialNPCId.Key);
            if (npc != null)
            {
                int count = 0;
                for (int i = 0; i < specialNPCId.Value.Count; i++)
                {
                    if (specialNPCId.Value[i] == true)
                        count++;
                    npc.starCount = count;
                }

                InventoryManager.inv.playerRepo.SpecialNPCRepo.Add(npc);
            }

        }

        foreach (string achId in playerData.unlockedAchievementIds)
        {
            AchievementData ach = InventoryManager.inv.gameRepo.AchievementRepo.Find(b => b.entryID == achId);
            if (ach != null)
                InventoryManager.inv.playerRepo.AchievementRepo.Add(ach);
        }

        //Buffs

        foreach (string buffID in playerData.unlockedBuffs.Keys)
        {
            BuffData buff = InventoryManager.inv.gameRepo.BuffsRepo.Find(b => b.id == buffID);
            if (buff != null)
                InventoryManager.inv.playerRepo.BuffsRepo.Add(buff);
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
                        if (playerData.dialogueFlags.TryGetValue(flag, out bool val))
                            playerData.dialogueFlags[flag] = val;
                        else
                            playerData.dialogueFlags.Add(flag, val);
                    }
                }
            }

            if (key == "unlockedSpecialCustomerIds")
            {
                Debug.Log("[DataManager] Processing unlockedSpecialCustomerIds update...");

                // Ensure target dictionary exists
                if (playerData.unlockedSpecialCustomerIds == null)
                {
                    playerData.unlockedSpecialCustomerIds = new Dictionary<string, List<bool>>();
                    Debug.Log("[DataManager] Initialized unlockedSpecialCustomerIds dictionary.");
                }

                // ✅ Assume value is already Dictionary<string, List<bool>>
                if (value is Dictionary<string, List<bool>> flags)
                {
                    Debug.Log($"[DataManager] Received {flags.Count} special customer entries.");

                    foreach (var kvp in flags)
                    {
                        string npcId = kvp.Key;
                        List<bool> stars = kvp.Value;

                        Debug.Log($"[DataManager] Processing NPC: {npcId} | Stars: [{string.Join(", ", stars)}]");
                        int starCount = stars.FindAll(b => b).Count;
                        Debug.Log($"[DataManager] Star count: {starCount}");

                        // Update local data
                        if (playerData.unlockedSpecialCustomerIds.ContainsKey(npcId))
                            playerData.unlockedSpecialCustomerIds[npcId] = new List<bool>(stars);
                        else
                            playerData.unlockedSpecialCustomerIds.Add(npcId, new List<bool>(stars));

                        // Sync with inventory
                        SpecialNPCData existingNpc = InventoryManager.inv.playerRepo.SpecialNPCRepo.Find(c => c.entryID == npcId);

                        if (existingNpc != null)
                        {
                            existingNpc.starCount = starCount;
                            Debug.Log($"[DataManager] Updated existing NPC: {npcId} → {starCount} stars");
                        }
                        else
                        {
                            SpecialNPCData template = InventoryManager.inv.gameRepo.SpecialNPCRepo.Find(c => c.entryID == npcId);
                            if (template != null)
                            {
                                SpecialNPCData newNpc = GameObject.Instantiate(template);
                                newNpc.starCount = starCount;
                                InventoryManager.inv.playerRepo.SpecialNPCRepo.Add(newNpc);
                                Debug.Log($"[DataManager] Added new NPC: {npcId} with {starCount} stars");
                            }
                            else
                            {
                                Debug.LogWarning($"[DataManager] No template found for NPC ID: {npcId}");
                            }
                        }
                    }
                }
                else
                {
                    // Emergency log: see what type actually came through
                    Debug.LogError($"[DataManager] Expected Dictionary<string, List<bool>>, but got: {value?.GetType()}");
                    if (value != null)
                        Debug.Log($"[DataManager] Value content: {JsonUtility.ToJson(value)}"); // May fail, but try
                }
            }

            if (key == "unlockedTermIds")
            {
                if (playerData.unlockedTermIds == null)
                    playerData.unlockedTermIds = new List<string>();

                if (value is List<string> locList)
                {
                    foreach (var val in locList)
                    {
                        playerData.unlockedTermIds.Add(val);

                        if (!InventoryManager.inv.playerRepo.TermRepo.Find(c => c.entryID == val))
                        {
                            TermData newTerm = InventoryManager.inv.gameRepo.TermRepo.Find(c => c.entryID == val);
                            if (newTerm != null)
                                InventoryManager.inv.playerRepo.TermRepo.Add(newTerm);
                        }
                    }
                }
            }

            if (key == "unlockedLocationIds")
            {
                if (playerData.unlockedLocationIds == null)
                    playerData.unlockedLocationIds = new List<string>();


                if (value is List<string> termList)
                {
                    foreach (var val in termList)
                    {
                        playerData.unlockedLocationIds.Add(val);

                        if (!InventoryManager.inv.playerRepo.LocationRepo.Find(c => c.entryID == val))
                        {
                            LocationData newLoc = InventoryManager.inv.gameRepo.LocationRepo.Find(c => c.entryID == val);
                            if (newLoc != null)
                                InventoryManager.inv.playerRepo.LocationRepo.Add(newLoc);
                        }
                    }
                }
            }

            if (key == "unlockedBuffs")
            {
                if (playerData.unlockedBuffs == null)
                    playerData.unlockedBuffs = new();

                if (value is Dictionary<string, int> flags)
                {
                    foreach (var flag in flags.Keys)
                    {
                        if (playerData.unlockedBuffs.ContainsKey(flag))
                            playerData.unlockedBuffs[flag] = flags[flag];
                        else
                            playerData.unlockedBuffs.Add(flag, flags[flag]);
                    }
                }
            }

            if (key == "unlockedRecipeIds")
            {
                if (value is List<string> recList)
                {
                    foreach (var rec in recList)
                    {
                        playerData.unlockedRecipeIds.Add(rec);
                        Recipe recipe = InventoryManager.inv.gameRepo.RecipeRepo.Find(c => c.id == rec);

                        if (recipe)
                            InventoryManager.inv.playerRepo.RecipeRepo.Add(recipe);
                    }
                }
            }

            if (key == "unlockedBeverageIds")
            {
                if (value is List<string> bevList)
                {
                    foreach (var bev in bevList)
                    {
                        playerData.unlockedBeverageIds.Add(bev);
                        Beverage beverage = InventoryManager.inv.gameRepo.BeverageRepo.Find(c => c.id == bev);

                        if (beverage)
                            InventoryManager.inv.playerRepo.BeverageRepo.Add(beverage);
                    }

                }
            }

            if (key == "unlockedCustomerIds")
            {
                if (value is List<string> cusList)
                {
                    foreach (var cus in cusList)
                    {
                        playerData.unlockedCustomerIds.Add(cus);
                        CustomerData customerData = InventoryManager.inv.gameRepo.CustomerRepo.Find(c => c.id == cus);
                        InventoryManager.inv.playerRepo.CustomerRepo.Add(customerData);
                    }
                }
            }

            if (key == "unlockedKitchenTools")
            {
                if (playerData.unlockedKitchenTools == null)
                    playerData.unlockedKitchenTools = new();

                if (value is Dictionary<string, int> flags)
                {
                    foreach (var flag in flags.Keys)
                    {
                        if (playerData.unlockedKitchenTools.ContainsKey(flag))
                            playerData.unlockedKitchenTools[flag] = flags[flag];
                        else
                            playerData.unlockedKitchenTools.Add(flag, flags[flag]);
                    }
                }
            }

            if (key == "day" && value is int day) playerData.day = day;
            if (key == "money" && value is float money) playerData.money = money;
            if (key == "voucher" && value is float voucher) playerData.voucher = voucher;
            if (key == "happiness" && value is float happiness) playerData.happiness = happiness;
            if (key == "lastLogin" && value is System.DateTime lastLogin) playerData.lastLogin = lastLogin;

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

    #endregion

}