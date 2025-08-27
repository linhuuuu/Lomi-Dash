using System;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;

namespace PCG
{
    public class BoilNode : OrderNode
    {
        public int time { get; set; } = 0;
        public int waterHeld { get; set; } = 0;

        public int tolerance = 2;

        public BoilNode(string id) => this.id = id;
        

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
            => $"[BOIL: {time}s ±{tolerance}s (w={weight:F1})]";
    }
}