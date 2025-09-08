namespace PCG
{
    public class DishSectionNode : OrderNode
    {
        public bool isLarge { set; get; }   //debugging

        public int index { set; get; } = 0;
        public DishSectionNode() => id = "DISH_SECTION";

        //public override float Evaluate(OrderNode other) => true;

    }
}