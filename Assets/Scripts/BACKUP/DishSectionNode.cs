using System.Collections;
using UnityEngine;

namespace PCG
{
    public class DishSectionNode : OrderNode
    {
        public DishSectionNode() => id = "DISH_SECTION";

        public override bool Matches(OrderNode other) => true;
    }
}