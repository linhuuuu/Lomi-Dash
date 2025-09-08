using Unity.VisualScripting;
using UnityEngine;

namespace PCG
{
    public class BeverageSectionNode : OrderNode
    {
        public int size { set; get; }

        public BeverageSectionNode(string id) => this.id = id;
        public BeverageSectionNode(Beverage beverage)
        {
            id = beverage.bevName;
            size = beverage.size;
        }
        public override float Evaluate(OrderNode other)
        {
            // if (!(other is BeverageSectionNode a)) //Checks if Beverage Node
            // {
            //     if (Debug.isDebugBuild) Debug.Log("Ordernode is not a BeverageNode");
            //     return 0f;
            // }

            // if (a.name == name && a.size == size) return weight;    //Checks if Bev matches and returns full weight

            return 0f;
        }
    }
}