using UnityEngine;

namespace PCG
{
    public class ToppingNode : OrderNode
    {
        public string toppingName { get; set; } = "";
        public int toppingCount { get; set; } = 0;
        public ToppingNode(string id)
        {
            this.id = id;
        } // clearer

        public override float Evaluate(OrderNode other)
        {
            // Must be a ToppingNode
            if (!(other is ToppingNode player))
            {
                if (Debug.isDebugBuild) Debug.Log($"[ToppingNode] Not a ToppingNode: {other?.GetType().Name}");
                return 0f;
            }

            // Names must match
            if (toppingName != player.toppingName)
                return 0f; // ❌ Wrong topping → no score

            // If player provided 0, no match
            if (player.toppingCount <= 0)
                return 0f;

            // Partial credit based on how many were added
            float ratio = Mathf.Min(player.toppingCount / (float)toppingCount, 1f);
            return ratio * weight;
        }

        public override string ToString()
            => $"[TOPPING: {toppingName} x{toppingCount} (w={weight:F1})]";
    }
}