using System.Collections;
using UnityEngine;

namespace PCG
{
    public class BeverageNode : OrderNode
    {
        public string drinkName;

        public BeverageNode()
        {
            id = "BEVERAGE";
        }

        public override bool Matches(OrderNode other)
        {
            return other is BeverageNode b && drinkName == b.drinkName;
        }

        public override string ToString()
        {
            return $"Beverage: {drinkName}";
        }
    }
}