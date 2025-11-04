using UnityEngine;
using PCG;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Unity.VisualScripting;
using System;
using System.Runtime.ConstrainedExecution;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    //Order Generation and Management
    public class Order
    {
        public OrderNode order;
        public CustomerGroup customers;
        public float price;
        public BuffData buff;
    }

    public Dictionary<int, Order> orders { set; get; } = new Dictionary<int, Order>();
    public OrderNode[] finishedOrders { set; get; }


    //Buff
    List<BuffData.Buff> moneyBuffs = new();
    List<BuffData.Buff> happinessBuffs = new();
    List<BuffData.Buff> timeBuffs = new();
    List<BuffData.Buff> priceBuffs = new();
    List<BuffData.Buff> scoreBuffs = new();
    public BuffData activeBuffData { set; get; } = null;

    [SerializeField] private int customersToCall = 0, currentOrders = 0;

    [Header("Round Profile")]
    private List<Beverage> beverages = new();
    private List<Recipe> recipes = new();
    private List<CustomerData> customerList = new();

    //Unlocks
    private Beverage unlockBeverage;
    private Recipe unlockRecipe;
    private List<CustomerData> unlockCustomer = new();
    private CustomerData specialCustomer = null;


    private RoundProfile profile;

    [Header("Round References and Objects")]

    //Customer
    private Transform customerPoolPoint;
    private List<CustomerSpawnPoint> customerSpawnPoints = new List<CustomerSpawnPoint>();
    [SerializeField] private GameObject customerPrefab;
    public List<DropObj> dropsToClaim { set; get; } = new();

    [SerializeField] private GameObject groupContainerPrefab;
    [SerializeField] private GameObject trayObjPrefab;
    [SerializeField] private GameObject dropPrefab;

    //Prompts
    [SerializeField] private GameObject promptPrefab;
    [SerializeField] private GameObject orderQueuePrefab;
    public Transform promptCanvas;
    public Transform orderQueue;

    //Prompts
    [SerializeField] private GameObject modalPrefab;
    [SerializeField] private GameObject modalCanvas;

    //Dialogue
    private List<string> eventsToPlay = new List<string>();

    [Header("Round Stats")]
    private float money = 0f;
    private float happiness = 0f;
    private int dishesCleared = 0;
    private int happyCustomers = 0;

    public event System.Action<float, float> OnCurrencyChange;
    private System.Random tempRNG;
    Dictionary<string, object> update = new Dictionary<string, object>();

    //Timer
    private float roundElapsedTime = 0f;
    private bool isTimerPaused = false;
    private bool isRoundActive = false;
    [SerializeField] Button toggleSpeed;
    private bool speed = true;

    public VisualStateLib lib;
    public static RoundManager roundManager;

    void Awake()
    {
        roundManager = this;
        if (GameManager.instance.state == GameManager.gameState.startDay)
            this.enabled = true;
        else
            this.enabled = false;
    }

    async void Start()
    {
        if (speed)
            Time.timeScale = 10;
        else
            Time.timeScale = 1;

        profile = GameManager.instance.roundProfile;

        // Set Available Data
        beverages = InventoryManager.inv.playerRepo.BeverageRepo;
        if (!beverages.Exists(c => c == profile.beverageUnlock))
        {
            beverages.Add(profile.beverageUnlock);
            unlockBeverage = profile.beverageUnlock;
        }

        recipes = new();
        foreach (Recipe recipe in profile.availableRecipes)
            if (InventoryManager.inv.playerRepo.RecipeRepo.Exists(c => c.id == recipe.id))
                recipes.Add(recipe);

        if (!recipes.Exists(c => c == profile.recipeUnlock))
        {
            recipes.Add(profile.recipeUnlock);
            unlockRecipe = profile.recipeUnlock;
        }

        foreach (CustomerData cus in profile.availableCustomers)
        {
            if (InventoryManager.inv.playerRepo.CustomerRepo.Exists(c => c == cus))
                customerList.Add(cus);
            else
                if (Debug.isDebugBuild) Debug.Log("Customer Data Missing. It is possible that customer data is missing from the repository or that the player has not yet unlocked this character.");
        }

        foreach (CustomerData cus in profile.customerUnlock)
            unlockCustomer.Add(cus);

        foreach (CustomerData cus in unlockCustomer)
        {
            if (customerList.Exists(c => c.id == cus.id))
                unlockCustomer.Remove(cus);
            else
                customerList.Add(cus);
        }

        //Set Spawn Points              
        for (int i = 0; i < 3; i++)
        {
            GameObject spawn = GameObject.Find("Spawn_" + (i + 1).ToString());
            if (spawn != null) customerSpawnPoints.Add(spawn.GetComponent<CustomerSpawnPoint>());
        }
        customerPoolPoint = GameObject.Find("customerPoolPoint").transform;
        for (int i = 0; i < customerSpawnPoints.Count; i++)
            customerSpawnPoints[i].index = i;

        //RNG
        ProceduralRNG.Initialize(profile.level);
        tempRNG = new System.Random((int)System.DateTime.Now.Ticks % int.MaxValue);

        //Init Round
        GenerateOrders();
        await StartRound();
    }

    #region Order Generation
    public void GenerateOrders()
    {
        int customerGroupCount = RoundManagerHelpers.helper.GenerateCustomerGroupCount(profile);
        List<Order> ordersList = new List<Order>();

        //SPECIAL CUSTOMER DESIGNATION
        specialCustomer = ChooseSpecialCustomer();
        if (Debug.isDebugBuild) Debug.Log(specialCustomer);
        // Choose Position
        int specialIndex = -1;
        if (specialCustomer != null)
            specialIndex = tempRNG.Next(0, customerGroupCount - 1);

        // GENERATE GROUPS
        for (int i = 0; i < customerGroupCount; i++)
        {
            Order newOrder = new Order();

            int headCount;
            OrderNode orderNode;
            float price;

            List<CustomerData> specialCustomerOverride = new();

            bool isSpecialGroup = specialCustomer != null && i == specialIndex;
            if (isSpecialGroup)
            {
                headCount = specialCustomer.companions.Count + 1;

                // Create special customer → override generation
                var result = OrderGenerator.GenerateTray(
                    difficulty: profile.difficulty,
                    headCount: 1,
                    largeBowlUnlocked: true,
                    bevList: specialCustomer.preferredBeverage,
                    recipeList: specialCustomer.preferredDish
                );

                orderNode = result.Item1;
                price = result.Item2;
                specialCustomerOverride.Add(specialCustomer);
                foreach (var companion in specialCustomer.companions)
                    specialCustomerOverride.Add(companion);
            }
            else
            {
                // Normal group
                headCount = RoundManagerHelpers.helper.GenerateHeadCount(profile);
                headCount = Mathf.Clamp(headCount, 1, 3);

                var result = OrderGenerator.GenerateTray(
                    profile.difficulty, headCount, true, beverages, recipes
                );
                orderNode = result.Item1;
                price = result.Item2;
            }

            newOrder.order = orderNode;
            newOrder.price = price;
            newOrder.customers = InstCustomerGroups(headCount, i, specialCustomerOverride);

            ordersList.Add(newOrder);
            customersToCall++;

            // Instantiate UI elements
            newOrder.customers.InstTray(trayObjPrefab, newOrder);
            newOrder.customers.InstPrompt(promptPrefab, customerPoolPoint);
            newOrder.customers.InstQueueOrder(orderQueuePrefab, customerPoolPoint);
            newOrder.customers.InstModal(modalPrefab, newOrder, modalCanvas);
        }

        for (int i = 0; i < ordersList.Count; i++)
        {
            orders[i] = ordersList[i];
        }

        finishedOrders = new OrderNode[orders.Count];
    }

    private CustomerGroup InstCustomerGroups(int headCount, int orderID, List<CustomerData> specialCustomerOverride)
    {
        //Create Container
        GameObject group = Instantiate(groupContainerPrefab, Vector3.zero, Quaternion.identity, customerPoolPoint);
        group.transform.localRotation = Quaternion.identity;

        //Generate Customers
        CustomerGroup customerGroup = group.GetComponent<CustomerGroup>();
        customerGroup.orderID = orderID;

        for (int j = 0; j < headCount; j++)
        {
            //Offset
            Vector2 circle = UnityEngine.Random.insideUnitCircle * 1f;
            Vector3 offset = new Vector3(circle.x, circle.y, 0f);

            //Init Customer
            GameObject newCustomer = Instantiate(customerPrefab, group.transform.position + offset, Quaternion.identity, group.transform);
            newCustomer.transform.localRotation = Quaternion.identity;

            //Generate a Customer
            Customer newCustomerProp = newCustomer.GetComponent<Customer>();
            if (specialCustomerOverride.Count > 0)
                newCustomerProp.InitCustomer(specialCustomerOverride[j]);
            else
                newCustomerProp.InitCustomer(customerList[ProceduralRNG.Range(0, customerList.Count)]);
            customerGroup.customers.Add(newCustomerProp);
        }

        foreach (Customer customer in customerGroup.customers)
            customerGroup.timer.totalTime += customer.patience;

        if (specialCustomerOverride.Count > 0)
            customerGroup.isSpecialGroup = true;

        return customerGroup;
    }

    CustomerData ChooseSpecialCustomer()
    {
        //Designated SpecialCustomer
        if (!InventoryManager.inv.playerRepo.SpecialNPCRepo.Exists(c => c == profile.specialCustomerUnlock))
        {
            specialCustomer = profile.specialCustomerUnlock;
            return profile.specialCustomerUnlock;
        }

        //Random SpecialCustomer    // If customer has been unlocked this round
        else if (profile.specialCustomers.Count > 0)
        {
            List<CustomerData> availableSpecialCustomers = new();

            foreach (CustomerData cus in profile.specialCustomers)
                if (InventoryManager.inv.playerRepo.SpecialNPCRepo.Exists(c => c == cus))
                    availableSpecialCustomers.Add(cus);

            if (availableSpecialCustomers.Count > 0)
                return availableSpecialCustomers[tempRNG.Next(0, availableSpecialCustomers.Count)];
        }
        return null;
    }

    #endregion
    #region Round Routine

    [System.Serializable]
    public class RoundTimingDebug
    {
        public float totalPatience;
        public float avgPatience;
        public float targetServiceTime; // Half of avg
        public float spawnWindow;       // 30% of service time
        public float finalInterval;     // After difficulty
    }

    public RoundTimingDebug timingDebug = new RoundTimingDebug();

    private IEnumerator RoundLoop()
    {
        isRoundActive = true;
        roundElapsedTime = 0f;

        // First customer: initial delay
        yield return new WaitForSeconds(ProceduralRNG.Range(5f, 15f));
        if (orders.Count > 0 && HasAvailableSpawnPoint())
        {
            CallCustomerGroup(orders[0].customers);
            currentOrders++;
        }

        // Dynamic rhythm setup
        float spawnInterval = GetTargetSpawnInterval();
        float difficultyMultiplier = 1.0f - (profile.difficulty - 1) * 0.08f;
        float baseDelay = Mathf.Clamp(spawnInterval * difficultyMultiplier, 15f, 60f);

        float nextSpawnTime = Time.time + baseDelay;

        // Remaining customers
        for (int i = 1; i < orders.Count; i++)
        {
            // 🔍 Check: are ALL spawn points FREE?
            if (!customerSpawnPoints.Any(c => c.occupied))
            {
                // ✅ YES: No need to wait — jump in with just hesitation
                float preSpawnDelay = ProceduralRNG.Range(0f, 5f);
                yield return new WaitForSeconds(preSpawnDelay);

                CallCustomerGroup(orders[i].customers);
                currentOrders++;

                // Reset next spawn time from now
                nextSpawnTime = Time.time + baseDelay;
            }
            else
            {
                // 🔁 NO: Some spawns occupied → follow normal timed rhythm
                while (Time.time < nextSpawnTime)
                    yield return null;

                // Wait for any spawn point to be available
                if (customerSpawnPoints.All(c => c.occupied))
                    yield return new WaitWhile(() => customerSpawnPoints.All(c => c.occupied));

                // Add natural hesitation before appearing
                float preSpawnDelay = ProceduralRNG.Range(0f, 5f);
                yield return new WaitForSeconds(preSpawnDelay);

                // Spawn!
                CallCustomerGroup(orders[i].customers);
                currentOrders++;

                // Schedule next
                nextSpawnTime += baseDelay;
            }
        }

        // Wait for all to leave and currency be picked up
        yield return new WaitWhile(() => currentOrders > 0);
        yield return new WaitWhile(() => dropsToClaim.Count > 0);


        OnRoundComplete();
        isRoundActive = false;
    }

    private bool HasAvailableSpawnPoint()
    {
        return customerSpawnPoints.Exists(c => !c.occupied);
    }

    private float GetTargetSpawnInterval()
    {
        // Total patience across all groups
        float totalPatience = 0f;
        int groupCount = orders.Count;

        foreach (var order in orders.Values)
        {
            totalPatience += order.customers.timer.totalTime;
        }

        float avgPatience = totalPatience / groupCount;

        // Target: player should finish by HALF of patience
        float targetServiceTime = avgPatience * 0.5f;

        // ✅ Spawn every 25% to 50% of service time → dynamic range
        float minSpawnInterval = targetServiceTime * 0.25f; // 1/4
        float maxSpawnInterval = targetServiceTime * 0.5f;  // 1/2

        // Use difficulty to slide between min and max
        float t = Mathf.InverseLerp(1, 5, profile.difficulty); // 0.0 (diff1) → 1.0 (diff5)
        float rawInterval = Mathf.Lerp(maxSpawnInterval, minSpawnInterval, t);

        // Add slight randomness so not robotic
        float jitterRange = rawInterval * 0.15f; // ±15%
        float finalInterval = rawInterval + ProceduralRNG.Range(-jitterRange, jitterRange);
        finalInterval = Mathf.Clamp(finalInterval, 15f, 60f);

        // 🔍 Debug
        timingDebug.totalPatience = totalPatience;
        timingDebug.avgPatience = avgPatience;
        timingDebug.targetServiceTime = targetServiceTime;
        timingDebug.spawnWindow = rawInterval;
        timingDebug.finalInterval = finalInterval;

        return finalInterval;
    }

    public void ToggleRoundTimer() => isTimerPaused = !isTimerPaused;

    void Update()
    {
        if (isRoundActive && !isTimerPaused)
        {
            roundElapsedTime += Time.unscaledDeltaTime;
        }
    }

    public async Task StartRound()
    {
        //Dialogue
        string roundName = GameManager.instance.roundProfile.roundName.ToString() + "_Before";

        if (!DataManager.data.playerData.dialogueFlags.TryGetValue(roundName, out bool played))
            if (Debug.isDebugBuild) Debug.Log("Key does not exist.");

        if (!played)
        {
            await DialogueManager.dialogueManager.PlayDialogue(roundName);
            Dictionary<string, object> updatedDialogueFlags = new Dictionary<string, object>
            {
                {roundName, true}
            };

            await DataManager.data.UpdatePlayerDataAsync(new Dictionary<string, object>
            {
                {"dialogueFlags", updatedDialogueFlags}
            });
        }

        //BGM 
        BGM bgm = profile.location switch
        {
            Locations.Lipa => BGM.LIPA,
            Locations.PadreGarcia => BGM.PADRE_GARICA,
            Locations.Batangas => BGM.BATANGAS,
            Locations.Mabini => BGM.MABINI,
            Locations.Taal => BGM.TAAL,
            _ => BGM.LIPA
        };
        AudioManager.instance.PlayBGM(bgm);

        //StartRound
        StartCoroutine(RoundLoop());
    }

    private async void OnRoundComplete()
    {
        bool isRoundFailed = money < profile.moneyQuota;

        //Total Score
        float totalScore = 0;
        foreach (OrderNode dish in finishedOrders)
            totalScore += dish.weight;
        float averageScore = Mathf.Clamp(totalScore / orders.Count, 0, 1) * 100;

        if (Debug.isDebugBuild) Debug.Log(totalScore);
        if (Debug.isDebugBuild) Debug.Log(averageScore);
        //Star
        int starCollected = 0;
        if (isRoundFailed)
            starCollected = 0;
        else if (averageScore <= 30)
            starCollected = 1;
        else if (averageScore > 30 && averageScore <= 60)
            starCollected = 2;
        else if (averageScore > 60)
            starCollected = 3;

        //Results
        DataManager.data.results = new RoundResults
        {
            userId = GameManager.instance.uid,
            totalDishes = this.orders.Count(),
            dishesCleared = this.dishesCleared,
            happyCustomers = this.happyCustomers,
            unhappyCustomers = orders.Count - this.happyCustomers,
            earnedHappiness = this.happiness,
            earnedMoney = this.money,
            clearDate = DateTime.Now,
            clearTime = roundElapsedTime,
            starCount = starCollected,
        };

        //Round Fail
        if (isRoundFailed)
        {
            GameManager.instance.NextScene("Results Screen");
            return;
        }

        //DATA SAVING
        if (unlockRecipe != null)
            update.Add("unlockedRecipeIds", new List<string> { unlockRecipe.id });

        if (unlockBeverage != null)
            update.Add("unlockedBeverageIds", new List<string> { unlockBeverage.id });

        if (unlockCustomer != null && unlockCustomer.Count > 0)
        {
            List<string> customerList = new();
            foreach (var customer in unlockCustomer)
                customerList.Add(customer.id);

            update.Add("unlockedCustomerIds", customerList);
        }

        //DIALOGUE
        string afterDialogue = $"{GameManager.instance.roundProfile.roundName}_After";
        if (DataManager.data.playerData.dialogueFlags.TryGetValue(afterDialogue, out bool dialogue) && dialogue == false)
        {
            AddToDialogueToPlay(afterDialogue);
            Dictionary<string, object> updatedDialogueFlags = new Dictionary<string, object> { { afterDialogue, true } };
            update.Add("dialogueFlags", updatedDialogueFlags);
        }

        //EntryUnlocks
        AddEntriesToUpdates(profile.entryUnlocks);

        //DAY END SCENE
        //StartCoroutine(PlayDayEnd(isFailed, starCount))

        //Play Dialogue
        if (eventsToPlay.Count > 0)
        {
            foreach (string play in eventsToPlay)
            {
                if (!DataManager.data.playerData.dialogueFlags.TryGetValue(play, out bool played))
                    if (Debug.isDebugBuild) Debug.Log("Key does not exist.");

                if (!played)
                    await DialogueManager.dialogueManager.PlayDialogue(play);
            }
        }

        //Goto
        await DataManager.data.UpdatePlayerDataAsync(update);
        GameManager.instance.NextScene("Results Screen");
    }

    // Round Controls
    // public void PauseRound() => isPaused = true;
    // public void ResumeRound() => isPaused = false;

    #endregion
    #region Customer Group Actions

    private void CallCustomerGroup(CustomerGroup customerGroup)
    {
        //Gets All customerSpawnPoints
        List<int> availableSpawnPoints = customerSpawnPoints.Where(c => c.occupied == false).Select(sp => sp.index).ToList();

        CustomerSpawnPoint spawn = customerSpawnPoints[availableSpawnPoints[ProceduralRNG.Range(0, availableSpawnPoints.Count)]];
        spawn.occupied = true;

        //Set Transform to Loc
        customerGroup.transform.SetParent(spawn.loc);
        customerGroup.transform.localPosition = Vector3.zero;
        customerGroup.transform.localRotation = Quaternion.identity;

        //Set References
        customerGroup.spawnPoint = spawn;
        StartCoroutine(customerGroup.timer.StartTimer());

        AudioManager.instance.PlaySFX(SFX.CUSTOMER_SPAWN);
    }

    public void OnCustomerGroupLeaveStanding(CustomerGroup group)
    {
        currentOrders--;
        orders[group.orderID].customers = null;
        finishedOrders[group.orderID] = (orders[group.orderID].order);
        group.transform.GetComponentInParent<CustomerSpawnPoint>().occupied = false;
    }

    public void OnCustomerGroupLeaveSitting(CustomerGroup group)
    {
        currentOrders--;
        orders[group.orderID].customers = null;
        finishedOrders[group.orderID] = (orders[group.orderID].order);
        group.tableDropZone.occupied = false;
    }

    public void OnCustomerGroupLeaveDined(CustomerGroup group)
    {
        if (activeBuffData != null)
        {
            moneyBuffs = activeBuffData.buffs.FindAll(c => c.type == BuffData.BuffType.moneyBuff);
            happinessBuffs = activeBuffData.buffs.FindAll(c => c.type == BuffData.BuffType.happinessBuff);
            timeBuffs = activeBuffData.buffs.FindAll(c => c.type == BuffData.BuffType.energyBuff);
            priceBuffs = activeBuffData.buffs.FindAll(c => c.type == BuffData.BuffType.priceBuff);
            scoreBuffs = activeBuffData.buffs.FindAll(c => c.type == BuffData.BuffType.scoreBuff);
        }

        float totalMoney = 0;
        float totalHappiness = 0;
        float finalScore = finishedOrders[group.orderID].weight;

        //Apply Price Buffs
        if (scoreBuffs != null)
        {
            foreach (var scoreBuff in scoreBuffs)
            {
                if (scoreBuff.addend > 0)
                    finalScore += scoreBuff.addend;
                if (scoreBuff.factor > 0f)
                    finalScore += finalScore * scoreBuff.factor;
                if (scoreBuff.subtrahend > 0f)
                    finalScore -= scoreBuff.subtrahend;
            }
        }

        float finalPrice = orders[group.orderID].price;

        //Apply Price Buffs
        if (priceBuffs != null)
        {
            foreach (var priceBuff in priceBuffs)
            {
                if (priceBuff.addend > 0)
                    finalPrice += priceBuff.addend;
                if (priceBuff.factor > 0f)
                    finalPrice += finalPrice * priceBuff.factor;
                if (priceBuff.subtrahend > 0f)
                    finalPrice -= priceBuff.subtrahend;
            }
        }

        float elapsedTime = group.timer.elapsedTime;

        //Apply Energy Buffs
        if (timeBuffs != null)
        {
            foreach (var timeBuff in timeBuffs)
            {
                if (timeBuff.addend > 0)
                    elapsedTime += timeBuff.addend;
                if (timeBuff.factor > 0f)
                    elapsedTime += elapsedTime * timeBuff.factor;
                if (timeBuff.subtrahend > 0f)
                    elapsedTime -= timeBuff.subtrahend;
            }
        }

        float timeLeft = elapsedTime / group.timer.totalTime;

        foreach (Customer customer in group.transform.GetComponentsInChildren<Customer>())
            totalHappiness += 5;

        if (timeLeft >= 0.5f)
        {
            totalMoney += Mathf.Max(finalScore * 0.01f * finalPrice * (1 + timeLeft));
            totalHappiness *= Mathf.Max(finalScore * 0.01f * (1 + timeLeft));
        }
        else
        {
            totalMoney += Mathf.Max(finalScore * 0.01f * finalPrice * timeLeft, 0);
            totalHappiness *= Mathf.Max(finalScore * 0.01f * timeLeft);
        }

        //Apply Money Buffs
        if (moneyBuffs != null)
        {
            foreach (var moneyBuff in moneyBuffs)
            {
                if (moneyBuff.addend > 0)
                    totalMoney += moneyBuff.addend;
                if (moneyBuff.factor > 0f)
                    totalMoney += elapsedTime * moneyBuff.factor;
                if (moneyBuff.subtrahend > 0f)
                    totalMoney -= moneyBuff.subtrahend;
            }
        }

        //Apply Happiness Buffs
        if (happinessBuffs != null)
        {
            foreach (var happinessBuff in happinessBuffs)
            {
                if (happinessBuff.addend > 0)
                    totalHappiness += happinessBuff.addend;
                if (happinessBuff.factor > 0f)
                    totalHappiness += elapsedTime * happinessBuff.factor;
                if (happinessBuff.subtrahend > 0f)
                    totalHappiness -= happinessBuff.subtrahend;
            }
        }

        totalHappiness = (float)Math.Round(totalHappiness, 0);
        totalMoney = (float)Math.Round(totalMoney, 0);

        //Reset Buffs
        moneyBuffs = new();
        happinessBuffs = new();
        timeBuffs = new();
        priceBuffs = new();
        scoreBuffs = new();
        activeBuffData = null;

        if (Debug.isDebugBuild) Debug.Log($"Total Money: {totalMoney}");
        if (Debug.isDebugBuild) Debug.Log($"Total Happiness: {totalHappiness}");

        bool isCurrencyDropped = InstCurrenciesDrop(group, finalScore, totalMoney, totalHappiness);
        bool isCEDropped = false;

        if (specialCustomer != null && group.isSpecialGroup == true)
            isCEDropped = InstCEDrop(group, finalScore);

        if (isCurrencyDropped || isCEDropped)
            group.tableDropZone.occupied = true;

        else
            group.tableDropZone.occupied = false;

        //Update Round
        currentOrders--;
        orders[group.orderID].customers = null;

        //Update Stats
        if (((finalPrice * 0.01f) + timeLeft) / 2 >= 0.5f)
            happyCustomers++;
        dishesCleared++;
    }

    #endregion
    #region Currency

    public bool InstCEDrop(CustomerGroup group, float finalScore)
    {
        int starsCollected = 0;

        if (finalScore < 50f)
        {
            specialCustomer = null;
            return false;
        }

        SpecialNPCData special = InventoryManager.inv.playerRepo.SpecialNPCRepo.Find(c => c.entryID == specialCustomer.id);
        if (special != null)
        {
            starsCollected = special.starCount;
            if (special.starCount == 3) return false;
        }

        Drop data = InventoryManager.inv.gameRepo.DropsRepo.Find(c => c.id == $"CE_{specialCustomer.id}_{starsCollected + 1}");
        if (!data)
        {
            if (Debug.isDebugBuild) Debug.Log("DropData not Found!");
            specialCustomer = null;
            return false;
        }

        //Instantiate
        GameObject dropObj = Instantiate(dropPrefab, Vector3.zero, Quaternion.identity, group.transform.parent.transform.Find("Table").Find("DropZone"));
        DropObj drop = dropObj.GetComponent<DropObj>();
        drop.dropData = data;
        drop.InitSprite();

        dropsToClaim.Add(drop);

        return true;
    }

    public void AddCE(CharacterEvent CE)
    {
        //Add To EndGame
        AddToDialogueToPlay(CE.id);
        AddEntriesToUpdates(CE.unlockEntryData);

        // Update Stars
        List<bool> starStat = new List<bool> { true, false, false };

        if (DataManager.data.playerData.unlockedSpecialCustomerIds.TryGetValue(specialCustomer.id, out List<bool> existing))
        {
            starStat = new List<bool>(existing);

            if (Debug.isDebugBuild) Debug.Log($"Old Stat: {starStat}");

            for (int i = 0; i < starStat.Count; i++)
            {
                if (!starStat[i])
                {
                    starStat[i] = true;
                    break;
                }
            }

            if (Debug.isDebugBuild) Debug.Log($"New Stat: {starStat}");
        }

        // Build update payload: merge into existing dict
        Dictionary<string, List<bool>> specialCustomerUpdate = new Dictionary<string, List<bool>>
        {
            {specialCustomer.id, starStat}
        };

        update.Add("unlockedSpecialCustomerIds", specialCustomerUpdate);
    }

    public bool InstCurrenciesDrop(CustomerGroup group, float finalScore, float money, float happiness)
    {
        bool isCurrencyDropped = false;
        DropObj drop = null;

        if (money > 0)
        {
            GameObject dropObj = Instantiate(dropPrefab, Vector3.zero, Quaternion.identity, group.transform.parent.transform.Find("Table").Find("DropZone"));
            drop = dropObj.GetComponent<DropObj>();


            Drop data = null;
            if (finalScore >= 75)
                data = InventoryManager.inv.gameRepo.DropsRepo.Find(c => c.id == "MoneyHigh");
            if (finalScore < 75 && finalScore >= 25)
                data = InventoryManager.inv.gameRepo.DropsRepo.Find(c => c.id == "MoneyMed");
            if (finalScore < 25)
                data = InventoryManager.inv.gameRepo.DropsRepo.Find(c => c.id == "MoneyLow");

            drop.dropData = data;
            drop.dropData.floatVal = money;

            if (drop != null)
            {
                drop.InitSprite();
                dropsToClaim.Add(drop);
            }

            isCurrencyDropped = true;
        }

        if (happiness > 0)
        {
            GameObject dropObj = Instantiate(dropPrefab, Vector3.zero, Quaternion.identity, group.transform.parent.transform.Find("Table").Find("DropZone"));
            drop = dropObj.GetComponent<DropObj>();

            drop.dropData = InventoryManager.inv.gameRepo.DropsRepo.Find(c => c.id == "Happiness");
            drop.dropData.floatVal = happiness;

            if (drop != null)
            {
                drop.InitSprite();
                dropsToClaim.Add(drop);
            }

            isCurrencyDropped = true;
        }
        return isCurrencyDropped;
    }

    public void AddCurrencies(float money, float happiness)
    {
        if (Debug.isDebugBuild) Debug.Log("Added" + money);
        if (Debug.isDebugBuild) Debug.Log("Added" + happiness);

        this.money += money;
        this.happiness += happiness;
        OnCurrencyChange?.Invoke(this.money, this.happiness);
    }

    public void AddToDialogueToPlay(string dialogueName) => eventsToPlay.Add(dialogueName);

    #endregion
    #region Buff

    public void AddCurrentBuff(BuffData buff) => activeBuffData = buff;

    #endregion

    #region Misc Helper Functions
    void AddEntriesToUpdates(List<AlmanacEntryData> unlockEntryDatas)
    {
        foreach (AlmanacEntryData data in unlockEntryDatas)
        {
            if (data is LocationData)
            {
                if (update.ContainsKey("unlockedLocationIds"))
                {
                    if (update["unlockedLocationIds"] is List<string> locationList)
                    {
                        if (!locationList.Exists(c => c == data.entryID))
                            locationList.Add(data.entryID);
                        update["unlockedLocationIds"] = locationList;
                    }
                }
                else
                {
                    update.Add("unlockedLocationIds", new List<string> { data.entryID });
                }
            }
            if (data is TermData)
            {
                if (update.ContainsKey("unlockedTermIds"))
                {
                    if (update["unlockedTermIds"] is List<string> termList)
                    {
                        if (!termList.Exists(c => c == data.entryID))
                            termList.Add(data.entryID);
                        update["unlockedTermIds"] = termList;
                    }
                }
                else
                {
                    update.Add("unlockedTermIds", new List<string> { data.entryID });
                }
            }
        }
    }
    #endregion

}