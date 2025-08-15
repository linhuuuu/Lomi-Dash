using System.Collections.Generic;
using UnityEngine;

//Each node contains a name, a weight, children(list) and Matches function that could be overloaded. THIS IS A TEMPLATE.
namespace PCG
{
    public abstract class OrderNode
    {
        public string id;
        public float weight = 1f;
        public List<OrderNode> children = new List<OrderNode>();

        public abstract bool Matches(OrderNode other);
    }
}