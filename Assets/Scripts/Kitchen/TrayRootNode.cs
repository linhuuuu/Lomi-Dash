using PCG;
using UnityEngine;

public class TrayRootNode : OrderNode
{
    public int currentDishWeight { get; set; } = 0;
    public int currentBeverageWeight { get; set; } = 0;

    public TrayRootNode() => id = "TRAY_ROOT";

    
}
