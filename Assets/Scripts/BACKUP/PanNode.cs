using System.Collections;
using UnityEngine;

namespace PCG
{
    public class PanNode : OrderNode
    {
        public PanNode() => id = "PAN";
        public override bool Matches(OrderNode other) => true;
    }
}