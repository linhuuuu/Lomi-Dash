using System.Collections.Generic;
using UnityEngine;

//OrderNode, Base Class for each node. Has a string, weight, children and virtual evaluate function that could be overriden.
namespace PCG
{
    public class OrderNode
    {
        public string id;
        public float weight { set; get; }
        public List<OrderNode> children = new List<OrderNode>();

        public virtual float Evaluate(OrderNode other)
        {
            float score = 0f;

            if (ValidateId(other) == false) return 0f;  //If not correct id

            if (other.children.Count < 0) return 0f;    //If no nodes

            foreach (var child in children)
            {
                //Find Node
                OrderNode otherchild = other.children.Find(c => c.id == child.id);

                //if no corresponding Node found
                if (otherchild == null)
                    return 0f;

                //if Node found
                score += child.Evaluate(otherchild);
            }
            return score;
        }

        private bool ValidateId(OrderNode other)
        {
            //Check Nodes
            if (Debug.isDebugBuild) Debug.Log("Evaluating " + id + " with " + other.id);

            if (id != other.id)
            {
                if (Debug.isDebugBuild) Debug.Log("Failed to evaluate Node " + id + "with Other Node " + other.id);
                return false;
            }
            return true;
        }

        public override string ToString() => $"[{id} (w={weight})"; // Debugging
    }
}