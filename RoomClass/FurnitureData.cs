namespace Furniture
{
    // TODO To think about ctor of FurnitureData and FurnitureDataFlags (at least id property uniqueness, like increment and so on)
    // In order to contract ctor params of GeneralFurniture

    public class FurnitureData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Length { get; set; }
        public int Height { get; set; }
        public string Zone { get; set; }
        public int ExtraLength { get; }
        public int ExtraHeight { get; }

        public FurnitureData(int id, string name, int length, int height, string zone, int extraLength = 0, int extraHeight = 0)
        {
            Id = id;
            Name = name;
            Length = length;
            Height = height;
            Zone = zone;
            ExtraLength = extraLength;
            ExtraHeight = extraHeight;
        }
    }
}
