namespace PCG
{
    [System.Serializable]
    public class WokNode: OrderNode
    {
        public WokNode (string id) => this.id = id;
        public override string ToString() => $"[{id} (w={weight})"; // Debugging
    }
}
