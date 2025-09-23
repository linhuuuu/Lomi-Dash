using UnityEngine;
namespace PCG
{
    public class BonesNode : OrderNode
    {
        public int count { set; get; } = 0;
        public int time { set; get; } = 0;
        private float weightRatio = 0.5f;

        public BonesNode() => id = "BONES_NDOE";
        public BonesNode(int count, int time)
        {
            id = "BONES_NDOE";
            this.count = count;
            this.time = time;
        }

        public override float EvaluateLeafNode(OrderNode other)
        {
            if (other is not BoilNode player)
                return 0f;

            float countRatio = Mathf.Clamp(player.count / count, 0, 1);
            float timeRatio = Mathf.Clamp(player.time / time, 0, 1);

            return (countRatio * (weightRatio * weight)) + (timeRatio * (weightRatio * weight));
        }

        public override string ToString() => $"[{id}: Count x{count}s  Time: {time}s (w={weight:F1})]";
    }
}