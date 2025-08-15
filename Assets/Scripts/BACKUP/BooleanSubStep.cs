using System.Collections;
using UnityEngine;

namespace PCG
{
    public class BooleanSubStep : OrderNode
    {
        public string name;
        public bool expectedValue;

        public override bool Matches(OrderNode other)
        {
            return other is BooleanSubStep b && name == b.name && expectedValue == b.expectedValue;
        }
    }
}