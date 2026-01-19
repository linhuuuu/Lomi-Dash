// Container Nodes = No Evaluation/ Pass only\

using UnityEngine;

namespace PCG
{
    public class DishSectionNode : OrderNode
    {
        public bool isLarge { set; get; }
        public string recipeName { set; get; }
        public int index { set; get; } = 0;
        public DishSectionNode() => this.id = "DISH_SECTION_";
        public DishSectionNode(int id) => this.id = "DISH_SECTION_" + id.ToString();
    }

    public class PotGroup : OrderNode
    {
        public PotGroup() => id = "POT_GROUP";
    }

    public class ToppingGroup : OrderNode
    {
        public ToppingGroup() => id = "TOPPING_GROUP";

        public override float Evaluate(OrderNode other)
        {
            Debug.Log("Evaluating: " + id + " with " + other.id);

            if (id != other.id)
                return 0f;

            if (!(other is ToppingGroup playerSection))
                return 0f;

            float score = 0f;

            foreach (var expectedChild in children)
            {
                if (expectedChild is ToppingNode expectedTopping)
                {
                    var match = FindMatchingTopping(expectedTopping.id, playerSection);
                    if (match != null)
                    {
                        score += expectedTopping.EvaluateLeafNode(match);
                    }
                }
            }

            int totalPlayerToppings = 0;
            int matchedTypes = 0;

            foreach (var playerChild in playerSection.children)
            {
                if (playerChild is ToppingNode playerTopping)
                {
                    totalPlayerToppings++;

                    if (children.Exists(e => e.id == playerTopping.id))
                        matchedTypes++;
                }
            }

            int extraTypes = totalPlayerToppings - matchedTypes;

            if (totalPlayerToppings > 0)
            {
                float ratio = Mathf.Clamp01((float)matchedTypes / totalPlayerToppings);
                score *= ratio;
            }
            else
            {
                score = 0f; 
            }

            if (Debug.isDebugBuild) Debug.Log($"ToppingGroup Score: {score}");
            return score;
        }

        private ToppingNode FindMatchingTopping(string id, ToppingGroup section)
        {
            foreach (var child in section.children)
            {
                if (child is ToppingNode t && t.id == id)
                    return t;
            }
            return null;
        }
    }

    public class WokGroup : OrderNode
    {
        public WokGroup() => id = "WOK_GROUP";
    }
}


