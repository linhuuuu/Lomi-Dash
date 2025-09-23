using UnityEngine;
namespace PCG
{
    public class EggNode : OrderNode
    {
        public int eggCount {get; set;}
        
        public EggNode() => id = "EGG_NODE";
        public EggNode(int eggCount)
        {
            id = "EGG_NODE";
            this.eggCount = eggCount;
        }

        public override float EvaluateLeafNode(OrderNode other)
        {
            if (other is not EggNode player)
                return 0;

            float eggRatio = Mathf.Clamp(player.eggCount / eggCount, 0, 1);

            return eggRatio * weight;
        }

        public override string ToString()
           => $"[{id}: Count x{eggCount} (w={weight:F1})]";
    }
}