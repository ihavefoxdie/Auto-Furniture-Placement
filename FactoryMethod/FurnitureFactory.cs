using Furniture;

namespace FactoryMethod
{
    public abstract class FurnitureFactory
    {

        public abstract GeneralFurniture GetFurniture(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags);
        public abstract GeneralFurniture GetFurniture();

    }
}


