using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace PCG
{
    public class OrderNode
    {
        public string id;
        public float weight { set; get; }
        public List<OrderNode> children = new List<OrderNode>();

        public float Evaluate(OrderNode other)
        {
            float score = 0f;

            Debug.Log("Evaluating: " + id + "with" + other.id);
            if (id != other.id)
                return 0f;

            //Check if child is leaf node;
            if (other.children.Count == 0)
                return EvaluateLeafNode(other);

            //Iterate over children
            for (int i = 0; i < other.children.Count; i++)
            {

                //Check if childcount exceeds
                if (i >= children.Count)
                    return 0f;

                OrderNode childToEvaluate = other.children[i];
                OrderNode localChild = children[i];

                //if ID matches continue structure, otherwise find Id as fallback
                if (localChild.id == childToEvaluate.id)
                    score += children[i].Evaluate(childToEvaluate);
                else
                {
                    childToEvaluate = FindOtherChild(localChild.id, other);
                    if (childToEvaluate != null)
                        score += localChild.Evaluate(childToEvaluate);
                }
            }
            return score;
        }

        public virtual float EvaluateLeafNode(OrderNode other) //to be overriden by leafnodes
        {
            if (Debug.isDebugBuild) Debug.Log("Not a Leaf Node, Something is wrong lol");
            return 0f;
        }

        protected OrderNode FindOtherChild(string idToFind, OrderNode other)
        {
            return other.children.Find(c => c.id == idToFind);
        }

        public override string ToString() => $"[{id} (w={weight})"; // Debugging
    }

    public class TrayRootNode : OrderNode
    {
        public TrayRootNode() => id = "TRAY_ROOT";
    }
}