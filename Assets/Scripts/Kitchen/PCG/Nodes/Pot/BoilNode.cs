using System.Data.Common;
using UnityEngine;

namespace PCG
{
    public class BoilNode : OrderNode
    {
        public int time { get; set; } = 0;
        public int waterHeld { get; set; } = 0;
        public int tolerance = 2;
        public BoilNode() => id = "BOIL_NODE";
        public BoilNode(int time, int waterHeld)
        {
            id = "BOIL_NODE";
            this.time = time;
            this.waterHeld = waterHeld;
        }
        public override float Evaluate(OrderNode other)
        {
            if (!(other is BoilNode player))
            {
                if (Debug.isDebugBuild) Debug.Log($"[BoilNode] Mismatched type: expected BoilNode, got {other.GetType().Name}");
                return 0f;
            }

            int diff = Mathf.Abs(time - player.time);

            if (diff <= tolerance)
            {
                return weight; // Full credit
            }

            // Partial credit based on distance
            float maxToleranceForPartial = tolerance * 2;
            if (diff <= maxToleranceForPartial)
            {
                float ratio = 1f - (diff - tolerance) / (float)(maxToleranceForPartial - tolerance);
                return ratio * weight;
            }

            return 0f; // Too far off
        }

        public override string ToString()
            => $"[{id}: Water x{waterHeld}, Boiled for: {time}s (w={weight:F1})]";
    }
}