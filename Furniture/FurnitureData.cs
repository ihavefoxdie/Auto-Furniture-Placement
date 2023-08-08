namespace Furniture
{
    // TODO To think about ctor of FurnitureData and FurnitureDataFlags (at least id property uniqueness, like increment and so on)
    // In order to contract ctor params of GeneralFurniture

    public class FurnitureData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Zone { get; set; }
        public int ExtraWidth { get; }              //Extra width applied to the base width for clearance area boundries
        public int ExtraHeight { get; }             //Same as ExtraWidth but applied for height

        public FurnitureData(int id, string name, int length, int height, string zone, int extraLength = 0, int extraHeight = 0)
        {
            Id = id;
            Name = name;
            Width = length;
            Height = height;
            Zone = zone;
            ExtraWidth = extraLength;
            ExtraHeight = extraHeight;
        }
    }
}
