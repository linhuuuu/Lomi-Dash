using PCG;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ToppingSectionNode : OrderNode
{

    public List<string> currentToppings = new List<string>();
    public ToppingSectionNode(string id) => this.id = id;

    private void SortChildren()
    {
        children = children.OrderBy(n => n.id).ToList();
    }
    public override float Evaluate(OrderNode other)
    {

        if (!(other is ToppingSectionNode playerSection))
        {
            if (Debug.isDebugBuild) Debug.Log("ToppingsSection: Type mismatch");
            return 0f;
        }


        float totalScore = 0f;

        // For each expected topping, find a matching one in player's dish
        foreach (var expectedNode in children)
        {
            if (expectedNode is ToppingNode expectedTopping)
            {
                bool foundMatch = false;

                foreach (var playerNode in playerSection.children)
                {
                    if (playerNode is ToppingNode playerTopping)
                    {
                        // Only evaluate if names match
                        if (playerTopping.toppingName == expectedTopping.toppingName)
                        {
                            totalScore += expectedTopping.Evaluate(playerTopping);
                            foundMatch = true;
                            break; // One match per expected topping
                        }
                    }
                }

                // If no match found → 0 for this topping
                if (!foundMatch)
                {
                    // Optionally: log missing topping
                    // if (Debug.isDebugBuild) Debug.Log($"Missing: {expectedTopping.toppingName}");
                }
            }
        }

        return totalScore;
    }
}