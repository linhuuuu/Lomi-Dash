using System;
using Unity.VisualScripting;
using UnityEngine;

namespace PCG
{
    public class BoilNode : OrderNode
    {
        public int time { get; set; }
        public int waterHeld { get; set; }

        public int tolerance = 2;

        public override float Evaluate(OrderNode other)
        {
            if (!(other is BoilNode player))
            {
                Debug.Log($"[BoilNode] Mismatched type: expected BoilNode, got {other.GetType().Name}");
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

        public static implicit operator BoilNode(BonesNode v)
        {
            throw new NotImplementedException();
        }
    }
}