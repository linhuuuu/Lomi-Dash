using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

//Barebones... Need to fix this 
namespace PCG
{
    public class Mix_2_Node : OrderNode
    {
        //DOUBLECHECK
         public bool isMixed { get; set; } = false;
        public Mix_2_Node (string id) => this.id = id;


        public override float Evaluate(OrderNode other)
        {
            if (!(other is SeasoningPotNode node))
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

        
        //public override string ToString()
        //    => $"[Salt: {saltCount}s PepperCount: {pepperCount} (w={weight:F1})]";
    }
}