using UnityEngine;

namespace PCG
{
    public class SeasoningTrayNode : OrderNode
    {
        public int trayCount { get; set;}
        public override float Evaluate(OrderNode other)
        {
            if (!(other is SeasoningTrayNode a)) //Checks if SeasoningTrayNode
            {
                Debug.Log("Ordernode is not a BeverageNode");
                return 0f;
            }

            if ( a.trayCount == trayCount) return weight;    //Checks if Bev matches and returns full weight

            return 0f;
        }
        public override string ToString() => $"[{id} (w={weight}]";  //Debugging
    }
}