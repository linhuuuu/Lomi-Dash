using UnityEngine; 
namespace PCG
{
    public class SoySauceNode : OrderNode
    {
        public int count { get; set; } = 0;

        public SoySauceNode() => id = "SOYSAUCE_NODE";
        public SoySauceNode(int count)
        {
            id = "SOYSAUCE_NODE";
            this.count = count;
        }

        public override float EvaluateLeafNode(OrderNode other)
        {
            if (other is not SoySauceNode player)
                return 0f;

            float sauceRatio = Mathf.Clamp(player.count / count, 0f, 1f);
            float score = sauceRatio * weight;

            if (Debug.isDebugBuild) Debug.Log(score);
            return score;
        }
        
        public override string ToString()
           => $"[{id}: SauceCount x{count} (w={weight:F1})]";
    }
}