using UnityEngine;
namespace PCG
{
    public class BoilNode : OrderNode
    {
        public int count { get; set; } = 0;
        public int time { get; set; } = 0;
        private float weightRatio = 0.5f;

        public BoilNode() => id = "BOIL_NODE";
        public BoilNode(int count, int time)
        {
            id = "BOIL_NODE";
            this.count = count;
            this.time = time;
        }

        public override float EvaluateLeafNode(OrderNode other)
        {
            if (other is not BoilNode player)
                return 0f;

            float waterRatio = Mathf.Clamp(player.count / count, 0, 1);
            float timeRatio = Mathf.Clamp(player.time / time, 0, 1);

            float score = (waterRatio * (weightRatio * weight)) + (timeRatio * (weightRatio * weight));
            if (Debug.isDebugBuild) Debug.Log(score);
            
            return score;
        }

        public override string ToString() => $"[{id}: Water x{count}, Time: {time}s (w={weight:F1})]";
    }
}