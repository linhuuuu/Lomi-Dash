using UnityEngine;

//Barebones... Need to fix this 
namespace PCG
{
    public class SoySauceNode : OrderNode
    {
        //DOUBLECHECK
        public int sauceCount { get; set; } = 0;
        public SoySauceNode() => id = "SOYSAUCE_NODE";

        public SoySauceNode(int sauceCount)
        {
            id = "SOYSAUCE_NODE";
            this.sauceCount = sauceCount;
        }
        public override float Evaluate(OrderNode other)
        {
            if (!(other is SauteeNode node))
            {
                if (Debug.isDebugBuild) Debug.Log($"[BonesNode] Type mismatch: got {other?.GetType().Name}");
                return 0f;
            }

            //if (node.saltCount == 0 && node.pepperCount == 0) return 0f;

            //return (saltCount/node.saltCount) * (weight * 0.5f) + (pepperCount / node.pepperCount) * (weight * 0.5f);
            ////saltcount and peppercount could overexceed and minus the weight...
            ///
            return 1f;
        }
        public override string ToString()
           => $"[{id}: SauceCount x{sauceCount} (w={weight:F1})]";
    }
}