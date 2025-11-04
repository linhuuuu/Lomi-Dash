using UnityEngine;

namespace PCG
{
    public class BeverageSectionNode : OrderNode
    {
        public int size { set; get; }
        public string name { set; get; }
        private float weightRatio = 0.5f;

        public BeverageSectionNode() => this.id = "BEVERAGE_SECTION_";

        public BeverageSectionNode(Beverage beverage, int id)
        {
            this.id = "BEVERAGE_SECTION_" + id.ToString();
            size = beverage.size;
            name = beverage.bevName;
        }

        public BeverageSectionNode(Beverage beverage)
        {
            this.id = "BEVERAGE_SECTION_";
            size = beverage.size;
            name = beverage.bevName;
        }

        public override float EvaluateLeafNode(OrderNode other)
        {
            if (other is not BeverageSectionNode player)
                return 0f;

            float sizeRatio = Mathf.Clamp(player.size / size, 0, 1);
            float nameRatio = player.name == name ? 1f : 0f;

            float score = (sizeRatio * (weightRatio * weight)) + (nameRatio* (weightRatio * weight));
            if (Debug.isDebugBuild) Debug.Log(score);

            return score;
        }


        public override string ToString() => $"[{id} (w={weight:F1})]";
    }
}