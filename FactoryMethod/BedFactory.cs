using Furniture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod
{
    public class BedFactory : FurnitureFactory
    {
        private readonly FurnitureData _furnitureData = new(7, "bed", 3, 4, "bed", 1, 1);
        private readonly FurnitureDataFlags _furnitureDataFlags = new();

        public BedFactory(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            _furnitureData = furnitureData;
            _furnitureDataFlags = furnitureDataFlags;
        }

        public BedFactory()
        {

        }


        public override GeneralFurniture GetFurniture(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            Bed bed = new(furnitureData,furnitureDataFlags);
            return bed;
        }

        public override GeneralFurniture GetFurniture()
        {
            Bed bed = new(_furnitureData, _furnitureDataFlags);
            return bed;
        }
    }
}
