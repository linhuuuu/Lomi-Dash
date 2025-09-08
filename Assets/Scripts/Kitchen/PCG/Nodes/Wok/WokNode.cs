namespace PCG
{
    [System.Serializable]
    public class WokGroup: OrderNode
    {
        public WokGroup () => id = "WOK_GROUP";
        public override string ToString() => $"[{id} (w={weight})"; // Debugging
    }
}
