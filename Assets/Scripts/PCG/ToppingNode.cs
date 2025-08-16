using UnityEngine;

namespace PCG
{
    public class ToppingNode : OrderNode
    {
        public string toppingName { get; set; } // better name
        public int expectedCount { get; set; } // clearer

        public override float Evaluate(OrderNode other)
        {
            // Must be a ToppingNode
            if (!(other is ToppingNode player))
            {
                Debug.Log($"[ToppingNode] Not a ToppingNode: {other?.GetType().Name}");
                return 0f;
            }

            // Names must match
            if (toppingName != player.toppingName)
                return 0f; // ❌ Wrong topping → no score

            // If player provided 0, no match
            if (player.expectedCount <= 0)
                return 0f;

            // Partial credit based on how many were added
            float ratio = Mathf.Min(player.expectedCount / (float)expectedCount, 1f);
            return ratio * weight;
        }

        public override string ToString()
            => $"[TOPPING: {toppingName} x{expectedCount} (w={weight:F1})]";
    }
}