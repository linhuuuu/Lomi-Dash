using UnityEngine;

namespace PCG
{
    public class BonesNode : OrderNode
    {
        public int count{ set; get; } = 0;

        public BonesNode() => id = "BONES_NDOE";

        public BonesNode(int count)
        {
            id = "BONES_NDOE";
            this.count = 1;
        }

        public override float Evaluate(OrderNode other)
        {
            if (!(other is BonesNode player))
            {
                if (Debug.isDebugBuild) Debug.Log($"[BonesNode] Type mismatch: got {other?.GetType().Name}");
                return 0f;
            }

            if (player.count == 0) return 0f;
            return (player.count / count) * weight;
        }

        public override string ToString()
            => $"[{id}: Count x{count}s (w={weight:F1})]";
    }
}