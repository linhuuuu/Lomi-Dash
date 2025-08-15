using System.Collections;
using UnityEngine;

namespace PCG
{
    public class PotNode : OrderNode
    {
        public PotNode() => id = "POT";
        public override bool Matches(OrderNode other) => true;
       }
}