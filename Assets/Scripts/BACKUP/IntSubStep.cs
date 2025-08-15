using System.Collections;
using UnityEngine;

namespace PCG
{
    public class IntSubStep : OrderNode
    {
        public string name;
        public int expectedValue;
        public int tolerance = 2;

        public override bool Matches(OrderNode other)
        {
            if (other is IntSubStep i)
            {
                return name == i.name && Mathf.Abs(expectedValue - i.expectedValue) <= tolerance;
            }
            return false;
        }
    }
}