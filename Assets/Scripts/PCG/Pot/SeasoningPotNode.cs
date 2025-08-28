using UnityEngine;

//Barebones... Need to fix this 
namespace PCG
{
    public class SeasoningPotNode : OrderNode
    {
        //DOUBLECHECK
        public int saltCount { get; set; } = 0;
        public int pepperCount { get; set; } = 0;
        public int bawangCount { get; set; } = 0;
        public SeasoningPotNode(string id) => this.id = id;
        public override float Evaluate(OrderNode other)
        {
            if (!(other is SeasoningPotNode node))
            {
                if (Debug.isDebugBuild) Debug.Log($"[BonesNode] Type mismatch: got {other?.GetType().Name}");
                return 0f;
            }

            if (node.saltCount == 0 && node.pepperCount == 0) return 0f;

            return (saltCount / node.saltCount) * (weight * 0.5f) + (pepperCount / node.pepperCount) * (weight * 0.5f);
            //saltcount and peppercount could overexceed and minus the weight...
        }
        public override string ToString()
            => $"[Salt: {saltCount}s PepperCount: {pepperCount} (w={weight:F1})]";
    }
}