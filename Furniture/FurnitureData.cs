namespace Furniture
{
    /*TODO To think about ctor of FurnitureData and FurnitureDataFlags (at least id property uniqueness, like increment and so on)
    In order to contract ctor params of GeneralFurniture*/

    public class FurnitureData : ICloneable
    {
        public int ID { get; set; }                 //ID of the furniture object
        public string Name { get; set; }
        public int Depth { get; set; }
        public int FrontWidth { get; set; }
        public string Zone { get; set; }            //String value for the zone this furniture object belongs to
        public int ExtraDepth { get; }              //Extra width applied to the base width for clearance area boundries
        public int ExtraWidth { get; }              //Same as ExtraDepth but applied for frontWidth
        public int ParentID { get; private set; }   //ID of the parent furniture object
        public int ChildIndex { get; set; }
        public int ParentIndex { get; set; }

        public FurnitureData(int id, string name, int depth, int frontWidth, string zone, int extraDepth = 0, int extraWidth = 0, int parentID = -1)
        {
            ID = id;
            Name = name;
            Depth = depth;
            FrontWidth = frontWidth;
            Zone = zone;

            ExtraDepth = extraDepth;
            ExtraWidth = extraWidth;
            ParentID = parentID;

            ParentIndex = -1;
            ChildIndex = -1;
        }

        public object Clone()
        {
            FurnitureData data = new(ID, Name, Depth, FrontWidth, Zone, ExtraDepth, ExtraWidth, ParentID)
            {
                ParentIndex = this.ParentIndex,
                ChildIndex = this.ChildIndex
            };
            return data;
        }
    }
}
