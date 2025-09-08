
using UnityEngine;

//Barebones... Need to fix this 
namespace PCG
{
    public class EggNode : OrderNode
    {
        //DOUBLECHECK
        public int eggCount { get; set; }
        public EggNode () => id = "EGG_NODE";
        public EggNode(int eggCount)
        {
            id = "EGG_NODE";
            this.eggCount = eggCount;
        }

        public override float Evaluate(OrderNode other)
        {
            if (!(other is EggNode node))
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
           => $"[{id}: Count x{eggCount} (w={weight:F1})]";
    }
}