namespace PCG
{
    public class BeverageSectionNode : OrderNode
    {
        public int size { set; get; }
        public string name { set; get; }

        public BeverageSectionNode() => this.id = "BEVERAGE_SECTION_";

        public BeverageSectionNode(Beverage beverage, int id)
        {
            this.id = "BEVERAGE_SECTION_"+ id.ToString();
            size = beverage.size;
            name = beverage.bevName;
        }
         public override float EvaluateLeafNode(OrderNode other)
        {
            if (other is not SeasoningTraySectionNode player)
                return 0f;

            return 1f;
        }

        public override string ToString() => $"[{id} (w={weight:F1})]";
    }
}