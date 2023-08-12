namespace Furniture
{
    public class Pouf : GeneralFurniture
    {

        public Pouf(int id, string name, int length, int height, string zone, bool ignoreWindows, int extraLength = 0, int extraHeight = 0, int nearWall = -1, bool parent = false, bool accessable = false, string? parentName = null) : base(id, name, length, height, zone, ignoreWindows, extraLength, extraHeight, nearWall, parent, accessable, parentName)
        {

        }


        public Pouf(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags) : base(furnitureData, furnitureDataFlags)
        {

        }

    }
}
