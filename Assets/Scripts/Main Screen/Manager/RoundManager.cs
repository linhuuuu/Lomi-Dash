using UnityEngine;
using PCG;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

//Component that manages Orders(Instantiate and Store) and Customers(Instantiate, Spawn, Actions)
public class RoundManager : MonoBehaviour
{
    //Order Generation and Management
    public class Order
    {
        public OrderNode order;
        public List<GameObject> customers;
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
    private Transform poolPoint;
    private List<CustomerSpawnPoint> spawnPoints = new List<CustomerSpawnPoint>();
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private GameObject groupContainerPrefab;
    private float money = 0f;
    private float happiness = 0f;

    public event System.Action<float, float> OnCurrencyChange;

    public static RoundManager roundManager;

    void Awake()
    {
        if (GameManager.Instance.state == GameManager.gameState.open)
            this.enabled = true;
    }

    void Start()
    {
        roundManager = this;

        //Set Available Data
        // beverages = DataManager.data.playerData.beverages;
        // recipes = DataManager.data.playerData.recipes;
        // customerList = DataManager.data.playerData.customerList;
        profile = GameManager.Instance.roundProfile;

        //Set Spawn Points              
        for (int i = 0; i < 3; i++)
        {
            GameObject spawn = GameObject.Find("Spawn_" + (i + 1).ToString());
            Debug.Log(spawn);
            spawnPoints.Add(spawn.GetComponent<CustomerSpawnPoint>());
        }

        poolPoint = GameObject.Find("PoolPoint").transform;

        for (int i = 0; i < spawnPoints.Count; i++)
            spawnPoints[i].index = i;

        //PCG
        ProceduralRNG.Initialize(profile.level);

        //Init Round
        GenerateOrders();
        for (int i = 0; i < orders.Count; i++)
            PrintTree(orders[i].order, " ");

        StartRound();
    }

    void PrintTree(OrderNode node, string indent)
    {
        Debug.Log($"{indent}└─ {node.id} (w={node.weight})");
        if (node.children != null)
        {
            foreach (var child in node.children)
            {
                if (child != null)
                    PrintTree(child, indent + "  ");
            }
        }
    }

    #region Order Generation
    public void GenerateOrders()
    {
        //Get group Count based on Round profile data
        int customerGroupcount = RoundManagerHelpers.helper.GenerateCustomerGroupCount(profile);

        //Generate CustomerGroups
        for (int i = 0; i < customerGroupcount; i++)
        {
            Order newOrder = new Order();

            //Generate Head Count -> Clamp to 3 if large tray is unlocked
            int headCount = RoundManagerHelpers.helper.GenerateHeadCount(profile);
            if (!DataManager.data.playerData.largeTrayUnlocked)
                headCount = Mathf.Clamp(headCount, 1, 3);

            //Generate Orders
            newOrder.order = OrderGenerator.GenerateTray(profile.difficulty, headCount, DataManager.data.playerData.largeBowlUnlocked, DataManager.data.playerData.largeTrayUnlocked, beverages, recipes);

            //Customer Group
            newOrder.customers = InstCustomerGroups(headCount, i);

            //Update List and Counter
            orders.Add(i, newOrder);    //id, ordergenerated
            customersToCall++;
        }
        finishedOrders = new OrderNode[orders.Count];
    }

    private List<GameObject> InstCustomerGroups(int headCount, int orderID)
    {
        //Create Container
        GameObject group = Instantiate(groupContainerPrefab, Vector3.zero, Quaternion.identity, poolPoint);
        group.transform.localRotation = Quaternion.identity;
        group.GetComponent<CustomerGroup>().orderID = orderID;

        // Generate Customers
        List<GameObject> customers = new List<GameObject>();
        for (int j = 0; j < headCount; j++)
        {
            //Offset
            Vector2 circle = UnityEngine.Random.insideUnitCircle * 1f;
            Vector3 offset = new Vector3(circle.x, circle.y, 0f);

            //Init Customer
            GameObject newCustomer = Instantiate(customerPrefab, group.transform.position + offset, Quaternion.identity, group.transform);
            newCustomer.transform.localRotation = Quaternion.identity;
            customers.Add(newCustomer);

            //Init Customers
            Customer newCustomerProp = newCustomer.GetComponent<Customer>();
            newCustomerProp.InitCustomer(customerList[ProceduralRNG.Range(0, customerList.Count)]); //Improve

        }
        return customers;
    }
    #endregion
    #region Round Routine
    private IEnumerator RoundLoop()
    {
        int i = 0;
        while (customersToCall > 0)
        {
            //Wait Until spawnPoints are available
            if (spawnPoints.All(c => c.occupied))
                yield return new WaitWhile(() => spawnPoints.All(c => c.occupied));

            //Call Customer Group
            float delay = ProceduralRNG.Range(1f, 5f);
            yield return new WaitForSeconds(delay); //delays
            CallCustomerGroup(orders[i].customers[0].transform.parent);

            //Update List
            customersToCall--;
            currentOrders++;

            i++;
        }

        //Wait until No More Customers
        if (currentOrders > 0)
            yield return new WaitWhile(() => currentOrders > 0);

        OnRoundComplete();
    }

    private void OnRoundComplete()
    {
        SceneManager.LoadScene("Results Screen");
        Debug.Log("Round Complete!");
    }

    // Round Controls
    // public void PauseRound() => isPaused = true;
    // public void ResumeRound() => isPaused = false;
    public void StartRound() => StartCoroutine(RoundLoop());

    #endregion
    #region Customer Group Actions

    private void CallCustomerGroup(Transform customerGroup)
    {
        //Gets all spawnpoints that are not occupied and lists their index
        List<int> availableSpawnPoints = spawnPoints.Where(c => c.occupied == false).Select(sp => sp.index).ToList();
        Debug.Log(String.Join(",", availableSpawnPoints));

        CustomerSpawnPoint spawn = spawnPoints[availableSpawnPoints[ProceduralRNG.Range(0, availableSpawnPoints.Count)]];
        spawn.occupied = true;

        //Set Transform to Loc
        customerGroup.SetParent(spawn.loc);
        customerGroup.localPosition = Vector3.zero;
        customerGroup.localRotation = Quaternion.identity;

        //Start Patience Timer
        customerGroup.GetComponent<CustomerGroup>().StartCustomerTimer();
    }

    public void OnCustomerGroupSit(CustomerGroup group)
    {
        group.transform.GetComponentInParent<CustomerSpawnPoint>().occupied = false; //reset Spawn point
    }

    public void OnCustomerGroupLeaveStanding(CustomerGroup group)
    {
        currentOrders--;
        orders[group.orderID].customers = null;
        group.transform.GetComponentInParent<CustomerSpawnPoint>().occupied = false; //reset Spawn point
    }

    public void OnCustomerGroupLeaveSitting(CustomerGroup group)
    {
        currentOrders--;
        orders[group.orderID].customers = null;
        group.GetComponentInParent<TableDropZone>().occupied = false;   //reset table
    }

    public void OnCustomerGroupLeaveDined(CustomerGroup group)
    {
        float totalMoney = 0;
        float totalHappiness = 0;
        float totalPrice = 0;
        float totalTimeLeft = group.timer.elapsedTime / group.timer.totalTime;

        foreach (Customer customer in group.transform.GetComponentsInChildren<Customer>())
        {
            totalPrice += customer.price;
            totalHappiness += 5;
        }

        if (totalTimeLeft >= 0.5f)
        {
            totalMoney = Mathf.Max(finishedOrders[group.orderID].weight * 0.01f * totalPrice * (1 + totalTimeLeft));
            totalHappiness *= 1 + totalTimeLeft;
        }

        else
        {
            totalMoney = Mathf.Max(finishedOrders[group.orderID].weight * 0.01f * totalPrice * totalTimeLeft, 0);
            totalHappiness *= totalTimeLeft;
        }

        Currency currency = group.GetComponentInChildren<Currency>();
        currency.money = totalMoney;
        currency.happiness = totalHappiness;
    }

    #endregion
    #region Currency

    public void AddCurrencies(float money, float happiness)
    {
        this.money += money;
        this.happiness += happiness;
        OnCurrencyChange.Invoke(this.money, this.happiness);
    }
    #endregion
}