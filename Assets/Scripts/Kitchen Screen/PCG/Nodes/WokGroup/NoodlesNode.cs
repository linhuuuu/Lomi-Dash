using UnityEngine;
namespace PCG
{
    public class NoodlesNode : OrderNode
    {
        public int count { get; set; } = 0;
        public int time { get; set; } = 0;
        private float weightRatio = 0.5f;

        public NoodlesNode() => id = "NOODLES_NODE";
        public NoodlesNode(int noodleCount, int cookTime)
        {
            id = "NOODLES_NODE";
            this.count = noodleCount;
            this.time = cookTime;
        }
        public override float EvaluateLeafNode(OrderNode other)
        {
            if (other is not NoodlesNode player)
                return 0f;

            float countRatio = Mathf.Clamp(player.count / count, 0, 1);
            float timeRatio = Mathf.Clamp(player.time / time, 0, 1);

            return (countRatio * (weight * weightRatio)) + (timeRatio * (weight * weightRatio));
        }

        public override string ToString()
           => $"[{id}: Count: {count}s time: {time} (w={weight:F1})]";

    }
}