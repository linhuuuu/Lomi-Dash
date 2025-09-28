using UnityEngine; 
namespace PCG
{
    public class ThickenerNode : OrderNode
    {
        public int count { get; set; }
        
        public ThickenerNode() => id = "THICKENER_NODE";
        public ThickenerNode(int thickenerCount)
        {
            id = "THICKENER_NODE";
            this.count = thickenerCount;
        }
        public override float EvaluateLeafNode(OrderNode other)
        {
            if (other is not ThickenerNode player)
                return 0f;

            float countRatio = Mathf.Clamp(player.count, 0f, 1f);

            return countRatio * weight;
        }

        public override string ToString()
           => $"[{id}: Count x{count} (w={weight:F1})]";
    }
}