using UnityEngine;
namespace PCG
{
    public class Mix_2_Node : OrderNode
    {
        public bool isMixed { get; set; } = false;
        
        public Mix_2_Node() => id = "MIX_1_NODE";
        public Mix_2_Node(bool isMixed)
        {
            id = "MIX_1_NODE";
            this.isMixed = isMixed; 
        }

        public override float EvaluateLeafNode(OrderNode other)
        {
            if (other is not Mix_2_Node player)
                return 0f;

            if (isMixed == false)
                return 0f;
            return weight;
        }

        public override string ToString()
           => $"[{id}: isMixed: {isMixed} (w={weight:F1})]";
    }
}