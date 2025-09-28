using UnityEngine;
namespace PCG
{
    public class SauteeNode : OrderNode
    {
        //DOUBLECHECK
        public int oilCount { get; set; } = 0;
        public int onionCount { get; set; } = 0;
        public int bawangCount { get; set; } = 0;
        public int oilTime { get; set; } = 0;
        public int onionTime { get; set; } = 0;
        public int bawangTime { get; set; } = 0;
        private float[] weightRatio = new float[6] { 0.1f, 0.2f, 0.2f, 0.1f, 0.2f, 0.2f };

        public SauteeNode() => id = "SAUTEE_NODE";
        public SauteeNode(int oilCount, int onionCount, int bawangCount, int cookTime)
        {
            id = "SAUTEE_NODE";
            this.oilCount = oilCount;
            this.onionCount = onionCount;
            this.bawangCount = bawangCount;
            this.oilTime = cookTime;
            this.onionTime = cookTime;
            this.bawangTime = cookTime;
        }
        
        public override float EvaluateLeafNode(OrderNode other)
        {
            if (other is not SauteeNode player)
                return 0f;

            float oilCountRatio = Mathf.Clamp(player.oilCount / oilCount, 0, 1);
            float onionCountRatio = Mathf.Clamp(player.onionCount / onionCount, 0, 1);
            float bawangCountRatio = Mathf.Clamp(player.oilCount / oilCount, 0, 1);
            float oilTimeRatio = Mathf.Clamp(player.oilTime / oilTime, 0, 1);
            float onionTimeRatio = Mathf.Clamp(player.onionTime / onionTime, 0, 1);
            float bawangTimeRatio = Mathf.Clamp(player.bawangTime / bawangTime, 0, 1);

            return (oilCountRatio * (weight * weightRatio[0])) + (onionCountRatio * (weight * weightRatio[1])) + (bawangCountRatio * (weight * weightRatio[2]))
            + (oilTimeRatio * (weight * weightRatio[3])) + (onionTimeRatio * (weight * weightRatio[4])) + (bawangTimeRatio * (weight * weightRatio[5]));
        }

        public override string ToString()
           => $"[{id}: OilCount x{oilCount} Time: {oilTime}s OnionCount x{onionCount} Time: {onionTime}s BawangCount x{bawangCount} Time: {bawangTime}s (w={weight:F1})]";
    }
}