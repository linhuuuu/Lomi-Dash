using UnityEngine;
using PCG;
public class RoundManagerHelpers : MonoBehaviour
{
    public static RoundManagerHelpers helper;
    void Awake()
    {
        helper = this;   
    }

    public int GenerateCustomerGroupCount(RoundProfile profile)
    {
        //if customerGroupCounts is overriden, use custom group counts
        if (profile.isGroupCountOverriden)
            return ProceduralRNG.Range(profile.minCustomerGroupCount, profile.maxCustomerGroupCount);
        else
            return ProceduralRNG.Range(4 + profile.difficulty, 5 + profile.difficulty); //Minimum 4 order, Maximum 6
    }
    public int GenerateHeadCount(RoundProfile profile)
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

}