using System.Collections;
using UnityEngine;

namespace PCG
{
    public class TrayRootNode : OrderNode
    {
        public TrayRootNode() => id = "TRAY_ROOT";
        public override bool Matches(OrderNode other) => true;
    }
}