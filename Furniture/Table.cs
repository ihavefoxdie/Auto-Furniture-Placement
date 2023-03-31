namespace Furniture
{
    public class Table : GeneralFurniture
    {

        public Table(int id, string name, int length, int height, string zone, bool ignoreWindows, int extraLength = 0, int extraHeight = 0, int nearWall = -1, int parent = -1, bool accessable = false, string? parentName = null) : base(id, name, length, height, zone, ignoreWindows, extraLength, extraHeight, nearWall, parent, accessable, parentName)
        {

        }


        public Table(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags) : base(furnitureData, furnitureDataFlags)
        {


        }

    }
}
