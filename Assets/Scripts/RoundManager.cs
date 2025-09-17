using UnityEngine;
using PCG;
using System.Collections.Generic;
using System;
using System.Linq;
using Unity.VisualScripting.Dependencies.Sqlite;
using System.Collections;

public class RoundManager : MonoBehaviour
{

    public class Order
    {
        public OrderNode order;
        public List<GameObject> customers;
    }

    public List<Order> orders { private set; get; } = new List<Order>();
    public List<Order> currentOrders { private set; get; } = new List<Order>();
    public List<Order> finishedOrders { private set; get; } = new List<Order>();

    [Header("Round Profile")]
    private List<Beverage> beverages;
    private List<Recipe> recipes;
    [SerializeField] private RoundProfile profile;

    [Header("Customer")]
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private GameObject groupContainerPrefab;
    [SerializeField] private List<Transform> customerCallPoint;
    [SerializeField] private Transform customerSpawnPoint;
    private List<CustomerData> customerList;

    [Header("Round Controls")]
    [SerializeField] private bool isRoundActive = false;
    [SerializeField] private bool isPaused = false;
    void Start()
    {
        //Set Data
        beverages = DataManager.data.playerData.beverages;
        recipes = DataManager.data.playerData.recipes;
        customerList = DataManager.data.playerData.customerList;    //maybe make this come from the round profile instead

        //PCG
        ProceduralRNG.Initialize(profile.level);
        GenerateOrders();
        StartRound();
    }

    public void GenerateOrders()
    {
        //Get group Count based on Round profile data
        int customerGroupcount = GenerateCustomerGroupCount(profile);

        //Generate CustomerGroups
        for (int i = 0; i < customerGroupcount; i++)
        {
            Order newOrder = new Order();

            //Generate Head Count
            int headCount = GenerateHeadCount(profile);
            if (!DataManager.data.playerData.largeTrayUnlocked)
                headCount = Mathf.Clamp(headCount, 1, 3); //Reduce to three if tray is not upgraded

            //Generate Orders
            newOrder.order = OrderGenerator.GenerateTray(profile.difficulty, headCount, DataManager.data.playerData.largeBowlUnlocked, DataManager.data.playerData.largeTrayUnlocked, beverages, recipes);

            //Instantiate Customer Group
            newOrder.customers = InstCustomerGroups(headCount);

            //Add to order list
            orders.Add(newOrder);
            Debug.Log(orders[0]);

        }
    }
    #region Start Round Routine

    public void StartRound()
    {
        if (!isRoundActive)
            StartCoroutine(RoundLoop());
    }

    private IEnumerator RoundLoop()
    {
        isRoundActive = true;

        while (orders.Count > 0)
        {
            // Wait between spawns
            float delay = ProceduralRNG.Range(1, 3);   //min and mix delay
            yield return new WaitForSeconds(delay);

            if (isPaused || customerCallPoint.Count == 0)
                yield return new WaitWhile(() => isPaused || customerCallPoint.Count == 0); //stops customer spawn while on pause.

            //Calls the latest customer group
            Debug.Log(orders[0].customers[0]);
            CallCustomerGroup(orders[0].customers[0].transform.parent);

            //Update List
            currentOrders.Add(orders[0]);
            orders.Remove(orders[0]);
        }
        OnRoundComplete();
    }

    // Round Controls
    public void PauseRound() => isPaused = true;
    public void ResumeRound() => isPaused = false;

    private void OnRoundComplete()
    {
        Debug.Log("Round complete!");
    }


    #endregion

    #region CustomerGroup Helpers
    private List<GameObject> InstCustomerGroups(int headCount)
    {
        //Create Container
        GameObject group = Instantiate(groupContainerPrefab, Vector3.zero, Quaternion.identity, customerSpawnPoint);
        group.transform.localPosition = Vector3.zero;
        group.transform.localRotation = Quaternion.identity;

        List<GameObject> customers = new List<GameObject>();
        for (int j = 0; j < headCount; j++)
        {
            //Offset
            Vector2 circle = UnityEngine.Random.insideUnitCircle * 0.5f;
            Vector3 offset = new Vector3(circle.x, circle.y, 0f);

            //Init Customer
            GameObject newCustomer = Instantiate(customerPrefab, group.transform.position + offset, Quaternion.identity, group.transform);
            newCustomer.transform.localRotation = Quaternion.identity;
            customers.Add(newCustomer);

            //Init Customers
            CustomerDrag newCustomerProp = newCustomer.GetComponent<CustomerDrag>();
            newCustomerProp.customerData = customerList[ProceduralRNG.Range(0, customerList.Count)]; //Improve
            newCustomerProp.InitCustomer();
        }
        return customers;
    }

    private void CallCustomerGroup(Transform customerGroup)
    {
        //Get spawn Loc randomly
        Transform callPoint = customerCallPoint[ProceduralRNG.Range(0, customerCallPoint.Count)];

        customerGroup.SetParent(callPoint);
        customerGroup.localPosition = Vector3.zero;

        customerCallPoint.Remove(callPoint);
    }

    private int GenerateCustomerGroupCount(RoundProfile profile)
    {
        //if customerGroupCounts is overriden, use custom group counts
        if (profile.isGroupCountOverriden)
            return ProceduralRNG.Range(profile.minCustomerGroupCount, profile.maxCustomerGroupCount);
        else
            return ProceduralRNG.Range(4 + profile.difficulty, 5 + profile.difficulty); //Minimum 4 order, Maximum 6
    }
    private int GenerateHeadCount(RoundProfile profile)
    {
        float[] weights = new float[5];

        if (HasValidWeights(profile.customerCountWeights))
        {
            weights = profile.customerCountWeights;
        }
        else
        {
            weights = ThemeBasedWeights(profile.theme, profile.difficulty);
        }

        return WeightedHeadCount(weights);
    }

    private float[] ThemeBasedWeights(GameTheme theme, int difficulty)
    {
        float[] baseWeights = new float[5] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
        switch (theme)
        {
            case GameTheme.Default:
                {
                    if (difficulty == 1)
                    {
                        baseWeights[0] = 5f;
                        baseWeights[1] = 4f;
                        baseWeights[2] = 2f;
                    }
                    else if (difficulty == 2)
                    {
                        baseWeights[0] = 1f;
                        baseWeights[1] = 5f;
                        baseWeights[2] = 5f;
                        baseWeights[3] = 1f;
                    }
                    else if (difficulty == 3)
                    {
                        baseWeights[2] = 4f;
                        baseWeights[3] = 5f;
                        baseWeights[4] = 4f;
                    }
                    else
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            int size = i + 1;
                            baseWeights[i] = 1f + Mathf.Max(0, difficulty - (6 - size)) * 1.5f;
                        }
                    }
                    break;
                }
        }
        return baseWeights;
    }

    private bool HasValidWeights(float[] customerCountWeights)
    {
        foreach (var weight in customerCountWeights)
            if (weight > 0) return true;
        return false;
    }

    private int WeightedHeadCount(float[] weights)
    {
        float total = 0;
        foreach (float w in weights) total += w;

        float rand = ProceduralRNG.Range(0f, total);
        float sum = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            sum += weights[i];
            if (rand <= sum)
                return 1 + i;
        }
        return 1;
    }
    #endregion
}