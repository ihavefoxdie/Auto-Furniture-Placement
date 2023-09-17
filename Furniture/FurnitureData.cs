namespace Furniture
{
    /*TODO To think about ctor of FurnitureData and FurnitureDataFlags (at least id property uniqueness, like increment and so on)
    In order to contract ctor params of GeneralFurniture*/

    public class FurnitureData : ICloneable
    {
        public int ID { get; set; }                 //ID of the furniture object
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Zone { get; set; }            //String value for the zone this furniture object belongs to
        public int ExtraWidth { get; }              //Extra width applied to the base width for clearance area boundries
        public int ExtraHeight { get; }             //Same as ExtraWidth but applied for height
        public int ParentID { get; private set; }   //ID of the parent furniture object
        public int? ChildIndex { get; set; }
        public int? ParentIndex { get; set; }

        public FurnitureData(int id, string name, int length, int height, string zone, int extraLength = 0, int extraHeight = 0, int parentID = -1)
        {
            ID = id;
            Name = name;
            Width = length;
            Height = height;
            Zone = zone;

            ExtraWidth = extraLength;
            ExtraHeight = extraHeight;
            ParentID = parentID;
        }

        public object Clone()
        {
            FurnitureData data = new(ID, Name, Width, Height, Zone, ExtraWidth, ExtraHeight, ParentID)
            {
                ParentIndex = ParentIndex != null ? (int)ParentIndex : null,
                ChildIndex = ParentIndex != null ? (int)ParentIndex : null
            };
            return data;
        }
    }
}
