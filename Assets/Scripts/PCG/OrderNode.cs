using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

//OrderNode, Base Class for each node. Has a string, weight, children and virtual evaluate function that could be overriden.
namespace PCG
{
    [System.Serializable]
    public class OrderNode
    {
        public string id;
        public float weight{set; get;}
        public List<OrderNode> children = new List<OrderNode>();
        public virtual float Evaluate(OrderNode other)
        {
            if (Debug.isDebugBuild) Debug.Log("Evaluating " + id + " with " +  other.id);
            //Check First if Container Matches.
            if (id != other.id)
            {
                if (Debug.isDebugBuild) Debug.Log("Failed to evaluate Node " + id + "with Other Node " + other.id);
                return 0f;
            }

            float score = 0f;
            for (int i = 0; i < children.Count; i++)
            {
                if (i < other.children.Count)
                {
                    score += children[i].Evaluate(other.children[i]);   //recursively call children score.
                   
                }
            }
            return score;
        }
        public override string ToString() => $"[{id} (w={weight})"; // Debugging
    }
}
