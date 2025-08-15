using UnityEngine;
using System.Collections.Generic;

/// Represents a single topping in a dish (e.g., Pork Belly, Egg, Green Onion)
namespace PCG
{
    public class ToppingNode : OrderNode
    {
        public string toppingName;

        public int expectedCount = 1;
        public ToppingNode() => id = $"TOPPING_{toppingName}";

        //Matches
        public override bool Matches(OrderNode other)
        {
            return other is ToppingNode t && toppingName == t.toppingName ;
        }

        //DEBUGGING PURPOSES
        public override string ToString()
        {
            return $"Topping: {toppingName} x{expectedCount} [Weight: {weight}]";
        }
    }
}