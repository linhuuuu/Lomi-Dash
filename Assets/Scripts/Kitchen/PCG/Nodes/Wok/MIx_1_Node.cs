
using UnityEngine;

//Barebones... Need to fix this 
namespace PCG
{
    public class Mix_1_Node : OrderNode
    {
        public bool isMixed { get; set; } = false;
        public Mix_1_Node () => id = "MIX_1_NODE";

        public Mix_1_Node(bool isMixed)
        {
            id = "MIX_1_NODE";
            this.isMixed = isMixed; 
        }

        public override float Evaluate(OrderNode other)
        {
            // if (!(other is SeasoningPotNode node))
            // {
            //     if (Debug.isDebugBuild) Debug.Log($"[BonesNode] Type mismatch: got {other?.GetType().Name}");
            //     return 0f;
            // }

            //if (node.saltCount == 0 && node.pepperCount == 0) return 0f;

            //return (saltCount / node.saltCount) * (weight * 0.5f) + (pepperCount / node.pepperCount) * (weight * 0.5f);
            ////saltcount and peppercount could overexceed and minus the weight...
            ///
            return 1f;
        }
        public override string ToString()
           => $"[{id}: isMixed: {isMixed} (w={weight:F1})]";
    }
}