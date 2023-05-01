namespace Furniture
{
    public class FurnitureDataFlags
    {
        public bool IgnoreWindows { get; protected set; }
        public int NearWall { get; protected set; }
        public int Parent { get; }
        public bool Accessible { get; protected set; }


        public FurnitureDataFlags(bool ignoreWindows = true, int nearWall = 1, int parent = -1, bool accessible = false)
        {
            IgnoreWindows = ignoreWindows;
            NearWall = nearWall;
            Parent = parent;
            Accessible = accessible;
        }

    }
}