using Furniture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod
{
    public class DoorFactory : FurnitureFactory
    {
        private readonly FurnitureData _furnitureData = new(10, "door", 3, 13, "obstacles", 30, 20);
        private readonly FurnitureDataFlags _furnitureDataFlags = new(false);

        public DoorFactory(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            _furnitureData = furnitureData;
            _furnitureDataFlags = furnitureDataFlags;
        }

        public DoorFactory()
        {

        }

        public override Door GetFurniture(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            Door door = new(furnitureData,furnitureDataFlags);
            return door;
        }

        public override Door GetFurniture()
        {
            Door door = new(_furnitureData, _furnitureDataFlags);
            return door;
        }
    }
}
