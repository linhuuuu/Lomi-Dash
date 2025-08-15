using System.Collections;
using UnityEngine;

namespace PCG
{
    public class ToppingsSectionNode : OrderNode
    {
        public ToppingsSectionNode() => id = "TOPPINGS_SECTION";
        public override bool Matches(OrderNode other) => true;
    }
}