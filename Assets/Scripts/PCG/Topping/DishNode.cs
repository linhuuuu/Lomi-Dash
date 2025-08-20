using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace PCG
{
    public class DishNode : OrderNode
    {
        public bool isLarge { get; set; } = false; //debugging
        public DishNode(string id) => this.id = id;
        //public override bool Matches(OrderNode other) => true;
    }
}