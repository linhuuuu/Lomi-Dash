using UnityEngine;

namespace PCG
{
    public class BonesNode : OrderNode
    {
        public int count{ get; set; }

        public override float Evaluate(OrderNode other)
        {
            if (!(other is BonesNode player))
            {
                Debug.Log($"[BonesNode] Type mismatch: got {other?.GetType().Name}");
                return 0f;
            }

            if (player.count == 0) return 0f;
           return (player.count / count) * weight;
        }

        public override string ToString()
            => $"[BOIL: {count}s (w={weight:F1})]";
    }
}