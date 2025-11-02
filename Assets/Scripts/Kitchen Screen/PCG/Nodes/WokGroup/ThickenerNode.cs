using UnityEngine;
namespace PCG
{
    public class ThickenerNode : OrderNode
    {
        public int count { get; set; }
        public bool isMixed { get; set; }

        private float weightRatio = 0.5f;

        public ThickenerNode() => id = "THICKENER_NODE";
        public ThickenerNode(int thickenerCount, bool isMixed)
        {
            id = "THICKENER_NODE";
            this.count = thickenerCount;
            this.isMixed = isMixed;
        }
        public override float EvaluateLeafNode(OrderNode other)
        {
            if (other is not ThickenerNode player)
                return 0f;

            float countRatio = Mathf.Clamp(player.count, 0f, 1f);
            float mixedRatio = isMixed ? 1 : 0;

            float score = (countRatio * (weightRatio * weight)) + (mixedRatio * (weightRatio * weight));
            if (Debug.isDebugBuild) Debug.Log(score);

            return score;
        }

        public override string ToString()
           => $"[{id}: Count x{count} (w={weight:F1})]";
    }
}