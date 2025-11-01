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
    private List<Beverage> beverages;
    private List<Recipe> recipes;
    private List<CustomerData> customerList;

    //SpecialCustomer
    private Beverage unlockBeverage;
    private Recipe unlockRecipe;
    private List<CustomerData> unlockCustomer;
    private CustomerData unlockSpecialCustomer;

    private RoundProfile profile;

    [Header("Round References and Objects")]

    //Customer
    private Transform customerPoolPoint;
    private List<CustomerSpawnPoint> customerSpawnPoints = new List<CustomerSpawnPoint>();
    [SerializeField] private GameObject customerPrefab;
    private CustomerData specialCustomer = null;

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
    private List<string> eventsToPlay;

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

        customerList = InventoryManager.inv.gameRepo.CustomerRepo;
        foreach (var customer in profile.customerUnlock)
        {
            if (!customerList.Exists(c => c == customer))
            {
                customerList.Add(customer);
                unlockCustomer.Add(customer);
            }
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
        Debug.Log(specialCustomer);
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

        // Convert to dictionary if needed
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

        return customerGroup;
    }

    CustomerData ChooseSpecialCustomer()
    {
        //Designeated SpecialCustomer
        if (InventoryManager.inv.playerRepo.SpecialNPCRepo.Exists(c => c.entryID == profile.specialCustomerUnlock.id))
        {
            unlockSpecialCustomer = profile.specialCustomerUnlock;
            return profile.specialCustomerUnlock;
        }

        //Random SpecialCustomer
        else if (profile.specialCustomers.Count > 0)
            return profile.specialCustomers[tempRNG.Next(0, profile.specialCustomers.Count)];

        return null;
    }

    #endregion
    #region Round Routine

    private IEnumerator RoundLoop()
    {
        // START TIMER 
        isRoundActive = true;
        roundElapsedTime = 0f;
        isTimerPaused = false;

        int i = 0;
        while (customersToCall > 0)
        {
            // Wait until there's a free spawn point
            if (customerSpawnPoints.All(c => c.occupied))
                yield return new WaitWhile(() => customerSpawnPoints.All(c => c.occupied));

            float delay = ProceduralRNG.Range(5f, 10f);
            yield return new WaitForSeconds(delay);

            CallCustomerGroup(orders[i].customers);

            customersToCall--;
            currentOrders++;
            i++;
        }

        if (currentOrders > 0)
            yield return new WaitWhile(() => currentOrders > 0);

        OnRoundComplete();

        // STOP TIMER 
        isRoundActive = false;
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

        //StartRound
        StartCoroutine(RoundLoop());
    }

    private async void OnRoundComplete()
    {
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
            starCount = 3, //adjust
        };

        //DATA SAVING

        if (unlockRecipe != null)
            update.Add("unlockedRecipeIds", unlockRecipe);

        if (unlockBeverage != null)
            update.Add("unlockedBeverageIds", unlockRecipe);

        if (unlockCustomer != null)
            update.Add("unlockedCustomerIds", unlockRecipe);

        //DIALOGUE
        string afterDialogue = $"{GameManager.instance.roundProfile.roundName}_After";
        if (DataManager.data.playerData.dialogueFlags[afterDialogue] == false)
        {
            AddToDialogueToPlay(afterDialogue);
            Dictionary<string, object> updatedDialogueFlags = new Dictionary<string, object> { { afterDialogue, true } };
            update.Add("dialogueFlags", updatedDialogueFlags);
        }

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

        if (dishesCleared > orders.Count / 2)
            foreach (AlmanacEntryData data in profile.entryUnlocks)
            {
                if (data is LocationData)
                    update.Add("unlockedLocationIds", data);
                if (data is TermData)
                    update.Add("unlockedTermIds", data);
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
    }

    public void OnCustomerGroupLeaveStanding(CustomerGroup group)
    {
        currentOrders--;
        orders[group.orderID].customers = null;
        group.transform.GetComponentInParent<CustomerSpawnPoint>().occupied = false;
    }

    public void OnCustomerGroupLeaveSitting(CustomerGroup group)
    {
        currentOrders--;
        orders[group.orderID].customers = null;
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

        //Reset Buffs
        moneyBuffs = new();
        happinessBuffs = new();
        timeBuffs = new();
        priceBuffs = new();
        scoreBuffs = new();
        activeBuffData = null;

        //Reset Table if no currency dropped. 
        Debug.Log(totalHappiness);
        bool isCurrencyDropped = InstCurrenciesDrop(group, finalScore, totalMoney, totalHappiness);

        bool isCEDropped = InstCEDrop(group, finalScore);

        if (isCurrencyDropped || isCEDropped)
        {
            group.tableDropZone.occupied = true;

            if (!isCEDropped)     //If specialcustomer has not been served well. reset specialcustomer unlock.
                specialCustomer = null;
        }

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
        //Skip if specialcustomer is not found or score is low
        if (specialCustomer == null) return false;
        if (!group.customers.Exists(c => c.data.id == specialCustomer.id)) return false;
        if (finalScore >= 0.5f) return false;

        //If custumer is new. Skip PlayerRepo Find.
        int starsCollected = 0;
        if (specialCustomer != unlockSpecialCustomer)
            starsCollected = InventoryManager.inv.playerRepo.SpecialNPCRepo.Find(c => c.entryID == specialCustomer.id).starCount;

        CharacterEvent CE;
        switch (starsCollected)
        {
            case 0:
                CE = specialCustomer.characterEvents[0];
                break;
            case 1:
                CE = specialCustomer.characterEvents[1];
                break;
            case 2:
                CE = specialCustomer.characterEvents[2];
                break;
            case 3:
                return false;
            default:
                return false;
        }

        GameObject dropObj = Instantiate(dropPrefab, Vector3.zero, Quaternion.identity, group.transform.parent.transform.Find("Table").Find("DropZone"));
        DropObj drop = dropObj.GetComponent<DropObj>();
        drop.dropData = InventoryManager.inv.gameRepo.DropsRepo.Find(c => c.id == "CE");
        drop.dropData.sprite = specialCustomer.portrait;
        drop.dropData.ceVal = CE;
        drop.dropData.id = specialCustomer.customerName;

        if (drop != null)
        {
            drop.InitSprite();
            return true;
        }
        else
        {
            Destroy(drop);
            return false;
        }
    }

    public void AddCE(CharacterEvent CE)
    {
        AddToDialogueToPlay(CE.id);

        foreach (AlmanacEntryData data in CE.unlockEntryData)
        {
            if (data is LocationData)
                update.Add("unlockedLocationIds", data);
            if (data is TermData)
                update.Add("unlockedTermIds", data);
        }

        // Update Stars
        List<bool> starStat = new List<bool> { true, false, false };
        if (unlockSpecialCustomer == null)  //if customer is not new
        {
            starStat = DataManager.data.playerData.unlockedSpecialCustomerIds[specialCustomer.id];
            for (int i = 0; i < starStat.Count; i++)
            {
                if (starStat[i] == false)
                {
                    starStat[i] = true;
                    break;
                }
            }
        }
        Dictionary<string, List<bool>> specialCustomerUpdate = new Dictionary<string, List<bool>> { { specialCustomer.id, starStat } };
        update.Add("unlockedSpecialCustomerID", specialCustomerUpdate);
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

            if (drop != null)
                drop.InitSprite();

            isCurrencyDropped = true;
        }

        if (happiness > 0)
        {
            GameObject dropObj = Instantiate(dropPrefab, Vector3.zero, Quaternion.identity, group.transform.parent.transform.Find("Table").Find("DropZone"));
            drop = dropObj.GetComponent<DropObj>();

            drop.dropData = InventoryManager.inv.gameRepo.DropsRepo.Find(c => c.id == "Happiness");

            if (drop != null)
                drop.InitSprite();

            isCurrencyDropped = true;
        }

        return isCurrencyDropped;
    }

    public void AddCurrencies(float money, float happiness)
    {
        Debug.Log("Added" + money);
        Debug.Log("Added" + happiness);
        this.money += money;
        this.happiness += happiness;
        OnCurrencyChange?.Invoke(this.money, this.happiness);
    }

    public void AddToDialogueToPlay(string dialogueName) => eventsToPlay.Add(dialogueName);

    #endregion
    #region Buff

    public void AddCurrentBuff(BuffData buff) => activeBuffData = buff;

    #endregion
}