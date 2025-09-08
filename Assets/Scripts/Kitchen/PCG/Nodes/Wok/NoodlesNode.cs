
using UnityEngine;

//Barebones... Need to fix this 
namespace PCG
{
    public class NoodlesNode : OrderNode
    {
        //DOUBLECHECK
        public int noodleCount { get; set; } = 0;
        public int cookTime { get; set; } = 0;
        public NoodlesNode() => id = "NOODLES_NODE";
        public NoodlesNode(int noodleCount, int cookTime)
        {
            id = "NOODLES_NODE";
            this.noodleCount = noodleCount;
            this.cookTime = cookTime;
        }
        public override float Evaluate(OrderNode other)
        {
            if (!(other is NoodlesNode node))
            {
                if (Debug.isDebugBuild) Debug.Log($"[BonesNode] Type mismatch: got {other?.GetType().Name}");
                return 0f;
            }

            //if (node.saltCount == 0 && node.pepperCount == 0) return 0f;

            //return (saltCount / node.saltCount) * (weight * 0.5f) + (pepperCount / node.pepperCount) * (weight * 0.5f);
            ////saltcount and peppercount could overexceed and minus the weight...
            ///
            return 1f;
        }
        
        public override string ToString()
           => $"[{id}: Count: {noodleCount}s CookTime: {cookTime} (w={weight:F1})]";
    }
}