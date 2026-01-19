using UnityEngine;
namespace PCG
{
    public class SeasoningNode : OrderNode
    {
        public int saltCount { get; set; } = 0;
        public int pepperCount { get; set; } = 0;
        private float weightRatio = 0.5f;

        public SeasoningNode() => id = "SEASONING_NODE";
        public SeasoningNode(int saltCount, int pepperCount)
        {
            id = "SEASONING_NODE";
            this.saltCount = saltCount;
            this.pepperCount = pepperCount;
        }

        public override float EvaluateLeafNode(OrderNode other)
        {
            if (other is not SeasoningNode player)
                return 0f;

            float saltRatio = Mathf.Clamp(player.saltCount / saltCount, 0, 1);
            float pepperRatio = Mathf.Clamp(player.pepperCount / pepperCount, 0, 1);

            float score = (saltRatio * (weight * weightRatio)) + (pepperRatio * (weight * weightRatio));
            if (Debug.isDebugBuild) Debug.Log(score);
            
            return score;
        }

        public override string ToString()
            => $"[{id}: Salt x{saltCount}s Pepper x{pepperCount} (w={weight:F1})]";
    }
}