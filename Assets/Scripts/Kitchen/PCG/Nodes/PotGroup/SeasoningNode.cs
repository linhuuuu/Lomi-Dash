using UnityEngine;
namespace PCG
{
    public class SeasoningNode : OrderNode
    {
        public int saltCount { get; set; } = 0;
        public int pepperCount { get; set; } = 0;
        public int bawangCount { get; set; } = 0;
        private float[] weightRatio = new float[3] { 0.3f, 0.3f, 0.4f };

        public SeasoningNode() => id = "SEASONING_NODE";
        public SeasoningNode(int saltCount, int pepperCount, int bawangCount)
        {
            id = "SEASONING_NODE";
            this.saltCount = saltCount;
            this.pepperCount = pepperCount;
            this.bawangCount = bawangCount;
        }

        public override float EvaluateLeafNode(OrderNode other)
        {
            if (other is not SeasoningNode player)
                return 0f;

            float saltRatio = Mathf.Clamp(player.saltCount / saltCount, 0, 1);
            float pepperRatio = Mathf.Clamp(player.pepperCount / pepperCount, 0, 1);
            float bawangRatio = Mathf.Clamp(player.bawangCount / bawangCount, 0, 1);

            return (saltRatio * (weight * weightRatio[0])) + (pepperRatio * (weight * weightRatio[1])) + (bawangRatio * (weight * weightRatio[1]));

        }

        public override string ToString()
            => $"[{id}: Salt x{saltCount}s Pepper x{pepperCount} Bawang x{bawangCount} (w={weight:F1})]";
    }
}