using System.Collections;
using UnityEngine;

namespace PCG
{
    public class SeasoningNode : OrderNode
    {
        public string seasoningType;

        public SeasoningNode()
        {
            id = "SEASONING";
        }

        public override bool Matches(OrderNode other)
        {
            return other is SeasoningNode s && seasoningType == s.seasoningType;
        }

        public override string ToString()
        {
            return $"Seasoning: {seasoningType}";
        }
    }
}