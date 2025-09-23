// Container Nodes = No Evaluation/ Pass only
namespace PCG
{
    public class DishSectionNode : OrderNode
    {
        public bool isLarge { set; get; }
        public int index { set; get; } = 0;
        public DishSectionNode() => this.id = "DISH_SECTION_";
        public DishSectionNode(int id) => this.id = "DISH_SECTION_" + id.ToString();
    }

    public class PotGroup : OrderNode
    {
        public PotGroup() => id = "POT_GROUP";
    }

    public class ToppingGroup : OrderNode
    {
        public ToppingGroup() => id = "TOPPING_GROUP";
    }

    public class WokGroup : OrderNode
    {
        public WokGroup() => id = "WOK_GROUP";
    }
}


