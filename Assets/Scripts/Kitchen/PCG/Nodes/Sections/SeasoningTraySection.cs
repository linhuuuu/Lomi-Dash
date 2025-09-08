using UnityEngine;

namespace PCG
{
    public class SeasoningTraySection: OrderNode
    {
        public int trayCount { get; set;}

        public SeasoningTraySection() => id = "SEASONING_TRAY_SECTION";
        public override float Evaluate(OrderNode other)
        {
            if (!(other is SeasoningTraySection a)) //Checks if SeasoningTrayNode
            {
                if (Debug.isDebugBuild) Debug.Log("Ordernode is not a BeverageNode");
                return 0f;
            }

            if (a.trayCount == trayCount) return weight;    //Checks if Bev matches and returns full weight

            return 0f;
        }
    }
}