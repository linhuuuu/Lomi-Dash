using UnityEngine;
namespace PCG
{
    public class EggNode : OrderNode
    {
        public int count { get; set; }
        public bool isMixed { get; set; }

        private float weightRatio = 0.5f;

        public EggNode() => id = "EGG_NODE";
        public EggNode(int count, bool isMixed)
        {
            id = "EGG_NODE";
            this.count = count;
            this.isMixed = isMixed;
        }

        public override float EvaluateLeafNode(OrderNode other)
        {
            if (other is not EggNode player)
                return 0;

            float eggRatio = Mathf.Clamp(player.count / count, 0, 1);
            float mixedRatio = isMixed ? 1 : 0;

            float score = (eggRatio * (weightRatio * weight)) + (mixedRatio * (weightRatio * weight));
            if (Debug.isDebugBuild) Debug.Log(score);
            return score;
        }

        public override string ToString()
           => $"[{id}: Count x{count} (w={weight:F1})]";
    }
}