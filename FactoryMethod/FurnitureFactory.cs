using Furniture;

namespace FactoryMethod
{
    //TODO: Discuss if we could make our factory a little cleared and more readable.
    public abstract class FurnitureFactory
    {

        public abstract GeneralFurniture GetFurniture(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags);
        public abstract GeneralFurniture GetFurniture();

    }
}


