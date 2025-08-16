using UnityEngine;

namespace PCG
{
    public class BeverageNode : OrderNode
    {
        public string name { set; get; }
        public int size { set; get; }
        public override float Evaluate(OrderNode other)
        {
            if(!(other is BeverageNode a)) //Checks if Beverage Node
            {
                Debug.Log("Ordernode is not a BeverageNode");
                return 0f;
            }

            if (a.name == name && a.size == size) return weight;    //Checks if Bev matches and returns full weight

            return 0f;
        }
        public override string ToString() => $"[{id} : {name} (w={weight}]";    //Debugging
    }
}