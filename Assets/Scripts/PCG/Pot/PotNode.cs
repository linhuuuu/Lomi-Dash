using System.Collections.Generic;
//OrderNode, Base Class for each node. Has a string, weight, children and virtual evaluate function that could be overriden.
namespace PCG
{
    [System.Serializable]
    public class PotNode : OrderNode
    {
        public PotNode(string id) => this.id = id;
        public override string ToString() => $"[{id} (w={weight})"; // Debugging
    }
}
