using UnityEngine;
namespace PCG
{
    public class ToppingNode : OrderNode
    {
        public int count { get; set; } = 0;

        public ToppingNode(string id) => this.id = id;
        public ToppingNode(Topping topping, int count)
        {
            id = topping.toppingName;
            this.count = count;
        }

        public override float EvaluateLeafNode(OrderNode other)
        {
            Debug.Log("Evaluating: " + id + " with " + other.id);
             
            if (other is not ToppingNode player)
                return 0f;

            float countRatio = Mathf.Clamp(player.count / count, 0f, 1f);
            float score = countRatio * weight;

            if (Debug.isDebugBuild) Debug.Log(score);
            return score;
        }

        public override string ToString()
            => $"[TOPPING: {id} x{count} (w={weight:F1})]";
    }
}