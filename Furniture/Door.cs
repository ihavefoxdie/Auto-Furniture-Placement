using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furniture
{
    public class Door : GeneralFurniture
    {
        public Door(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags) : base(furnitureData, furnitureDataFlags)
        {

        }

        public Door(int id, string name, int length, int height, string zone, bool ignoreWindows, int extraLength = 0, int extraHeight = 0, int nearWall = -1, bool parent = false, bool accessable = false, string? parentName = null) : base(id, name, length, height, zone, ignoreWindows, extraLength, extraHeight, nearWall, parent, accessable, parentName)
        {

        }


    }
}
