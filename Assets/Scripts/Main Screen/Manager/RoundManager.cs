using UnityEngine;
using PCG;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Unity.VisualScripting;
using System;

public class RoundManager : MonoBehaviour
{
    //Order Generation and Management
    public class Order
    {
        public OrderNode order;
        public CustomerGroup customers;
        public float price;
    }
    public Dictionary<int, Order> orders { set; get; } = new Dictionary<int, Order>();
    public OrderNode[] finishedOrders { set; get; }
    [SerializeField] private int customersToCall = 0, currentOrders = 0;

    [Header("Round Profile")]
    private List<Beverage> beverages;
    private List<Recipe> recipes;
    private List<CustomerData> customerList;
    private RoundProfile profile;

    [Header("Round References and Objects")]

    //Customer
    private Transform customerPoolPoint;
    private List<CustomerSpawnPoint> customerSpawnPoints = new List<CustomerSpawnPoint>();
    [SerializeField] private GameObject customerPrefab;

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

    [Header("Round Stats")]
    private float money = 0f;
    private float happiness = 0f;
    private int dishesCleared = 0;
    private int happyCustomers = 0;

    public event System.Action<float, float> OnCurrencyChange;

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

        Time.timeScale = 4f;
    }

    async void Start()
    {
        // Set Available Data
        beverages = InventoryManager.inv.gameRepo.BeverageRepo;
        recipes = InventoryManager.inv.gameRepo.RecipeRepo;
        customerList = InventoryManager.inv.gameRepo.CustomerRepo;
        profile = GameManager.instance.roundProfile;

        //Set Spawn Points              
        for (int i = 0; i < 3; i++)
        {
            GameObject spawn = GameObject.Find("Spawn_" + (i + 1).ToString());
            if (spawn != null) customerSpawnPoints.Add(spawn.GetComponent<CustomerSpawnPoint>());
        }

        customerPoolPoint = GameObject.Find("customerPoolPoint").transform;

        for (int i = 0; i < customerSpawnPoints.Count; i++)
            customerSpawnPoints[i].index = i;

        //PCG
        ProceduralRNG.Initialize(profile.level);

        //Init Round
        GenerateOrders();
        await StartRound();
    }

    #region Order Generation
    public void GenerateOrders()
    {
        //Get Group Count based on Round profile data
        int customerGroupcount = RoundManagerHelpers.helper.GenerateCustomerGroupCount(profile);

        //Generate CustomerGroups
        for (int i = 0; i < customerGroupcount; i++)
        {
            Order newOrder = new Order();

            //Generate Head Count -> Clamp to 3 if large tray is unlocked
            int headCount = RoundManagerHelpers.helper.GenerateHeadCount(profile);
            headCount = Mathf.Clamp(headCount, 1, 3);   //check if need large bowl and stuff

            //Generate Orders
            var orderGenerated = OrderGenerator.GenerateTray(profile.difficulty, headCount, true, true, beverages, recipes);    //large bowl and tray unclocked
            newOrder.order = orderGenerated.Item1;

            //Get Price
            newOrder.price = orderGenerated.Item2;

            //Customer Group
            newOrder.customers = InstCustomerGroups(headCount, i);

            //Update List and Counter
            orders.Add(i, newOrder);    //id, ordergenerated
            customersToCall++;

            // Instantiate Prompts and UITray
            newOrder.customers.InstTray(trayObjPrefab, orders[i]);
            newOrder.customers.InstPrompt(promptPrefab, customerPoolPoint);
            newOrder.customers.InstQueueOrder(orderQueuePrefab, customerPoolPoint);
            newOrder.customers.InstModal(modalPrefab, newOrder, modalCanvas);
        }
        finishedOrders = new OrderNode[orders.Count];
    }

    private CustomerGroup InstCustomerGroups(int headCount, int orderID)
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

            Customer newCustomerProp = newCustomer.GetComponent<Customer>();
            newCustomerProp.InitCustomer(customerList[ProceduralRNG.Range(0, customerList.Count)]); //Improve
            customerGroup.customers.Add(newCustomerProp);
        }
        foreach (Customer customer in customerGroup.customers)
            customerGroup.timer.totalTime += customer.patience;


        return customerGroup;
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

        if (DataManager.data.playerData.dialogueFlags.TryGetValue(roundName, out bool played)) 
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

    private void OnRoundComplete()
    {
        // for()
        
        //Results Saving
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
            clearTime = roundElapsedTime * 1000,
        
            starCount = 3,
        };

        //Goto
        GameManager.instance.ResultsScreen();
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
        float totalMoney = 0;

        float totalHappiness = 0;

        float finalScore = finishedOrders[group.orderID].weight;

        float finalPrice = orders[group.orderID].price;

        float timeLeft = group.timer.elapsedTime / group.timer.totalTime;

        foreach (Customer customer in group.transform.GetComponentsInChildren<Customer>())
            totalHappiness += 5;

        if (timeLeft >= 0.5f)
        {
            totalMoney = Mathf.Max(finalScore * 0.01f * finalPrice * (1 + timeLeft));
            totalHappiness *= Mathf.Max(finalScore * 0.01f * (1 + timeLeft));
        }
        else
        {
            totalMoney = Mathf.Max(finalScore * 0.01f * finalPrice * timeLeft, 0);
            totalHappiness *= Mathf.Max(finalScore * 0.01f * timeLeft);
        }

        //Reset Table if no currency dropped
        if (!InstCurrenciesDrop(group, finalScore, totalMoney, totalHappiness))
            group.tableDropZone.occupied = false;


        // InstToppingsDrop(group);
        // InstToppingDrop(group);

        //Update Round
        currentOrders--;
        orders[group.orderID].customers = null;

        if (((finalPrice * 0.01f) + timeLeft) / 2 >= 0.5f)
            happyCustomers++;
        dishesCleared++;
    }

    #endregion
    #region Currency

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
        this.money += money;
        this.happiness += happiness;
        OnCurrencyChange?.Invoke(this.money, this.happiness);
    }

    public void AddToppings(string id, int val)
    {
        //InventoryManager randomly
    }

    public void AddCE(string id)
    {
        //InventoryManager
    }

    #endregion
}