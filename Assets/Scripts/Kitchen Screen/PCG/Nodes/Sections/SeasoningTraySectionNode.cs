using UnityEngine;

namespace PCG
{
    public class SeasoningTraySectionNode : OrderNode
    {
        public int trayCount { get; set; }

        public SeasoningTraySectionNode() => id = "SEASONING_TRAY_SECTION";
        public SeasoningTraySectionNode(int trayCount)
        {
            this.trayCount = trayCount;
        }

        public override float EvaluateLeafNode(OrderNode other)
        {
            if (other is not SeasoningTraySectionNode player)
                return 0f;

            float score = Mathf.Clamp(player.trayCount / trayCount, 0, 1) * weight;

            if (Debug.isDebugBuild) Debug.Log(score);
            return score;
        }

        public override string ToString() => $"[{id}: Count x{trayCount}s (w={weight:F1})]";
    }
}