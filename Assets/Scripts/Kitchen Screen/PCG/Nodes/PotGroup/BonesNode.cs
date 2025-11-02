using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
namespace PCG
{
    public class BonesNode : OrderNode
    {
        public int count { set; get; } = 0;

        public BonesNode() => id = "BONES_NODE";
        public BonesNode(int count)
        {
            id = "BONES_NODE";
            this.count = count;
        }

        public override float EvaluateLeafNode(OrderNode other)
        {
            if (other is not BonesNode player)
                return 0f;

            float score = Mathf.Clamp(player.count / count, 0f, 1f) * weight;
            if (Debug.isDebugBuild) Debug.Log(score);
            return score;
        }

        public override string ToString() => $"[{id}: Count x{count}s (w={weight:F1})]";
    }
}