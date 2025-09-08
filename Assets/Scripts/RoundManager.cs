using UnityEngine;
using PCG;
using System.Collections.Generic;

public class RoundManager : MonoBehaviour
{
    public List<OrderNode> orders { private set; get; }
    [SerializeField] private List<Beverage> beverages = new List<Beverage>();
    [SerializeField] private List<Recipe> recipes = new List<Recipe>();
    [SerializeField] private RoundProfile profile;
    public PlayerData playerData;

    void Start()
    {
        //Init from playerData
        playerData = DataManager.data.playerData;

        //PCG
        ProceduralRNG.Initialize(System.DateTime.Now.GetHashCode());
        GenerateOrders();
    }

    public void GenerateOrders()
    {
        orders = new List<OrderNode>();
        int customerGroupcount = GenerateCustomerGroupCount(profile);
        for (int i = 0; i < customerGroupcount; i++)
        {
            int headCount = GenerateHeadCount(profile);

            if (!playerData.largeTrayUnlocked)
                headCount = Mathf.Clamp(headCount, 1, 3);

            var order = OrderGenerator.GenerateTray(profile.difficulty, headCount, playerData.largeBowlUnlocked, playerData.largeTrayUnlocked, beverages, recipes);
            orders.Add(order);
        }
    }

    private int GenerateCustomerGroupCount(RoundProfile profile)
    {
        //if customerGroupCounts is overriden, use custom group counts
        if (profile.isGroupCountOverriden)
            return ProceduralRNG.Range(profile.minCustomerGroupCount, profile.maxCustomerGroupCount);
        else
            return ProceduralRNG.Range(2 + profile.difficulty, 5 + profile.difficulty);
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
                    // Now boost the target range based on difficulty
                    if (difficulty == 1)
                    {
                        // Favor 1–3
                        baseWeights[0] = 5f; // 1 person
                        baseWeights[1] = 4f; // 2 people
                        baseWeights[2] = 2f; // 3 people
                    }
                    else if (difficulty == 2)
                    {
                        // Favor 2–4
                        baseWeights[0] = 1f; // 1
                        baseWeights[1] = 5f; // 2
                        baseWeights[2] = 5f; // 3 
                        baseWeights[3] = 1f; // 4
                    }
                    else if (difficulty == 3)
                    {
                        // Favor 3–5
                        baseWeights[2] = 4f; // 3
                        baseWeights[3] = 5f; // 4 
                        baseWeights[4] = 4f; // 5
                    }
                    else
                    {
                        // // For difficulty > 3, smoothly shift toward larger groups
                        // float shift = difficulty - 1; // 2, 3, ...
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
}