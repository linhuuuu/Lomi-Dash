using System.Collections.Generic;
//OrderNode, Base Class for each node. Has a string, weight, children and virtual evaluate function that could be overriden.
namespace PCG
{
    [System.Serializable]
    public class PotGroup : OrderNode
    {
        public PotGroup() => id = "POT_GROUP";
        public override string ToString() => $"[{id} (w={weight})"; // Debugging
    }
}
