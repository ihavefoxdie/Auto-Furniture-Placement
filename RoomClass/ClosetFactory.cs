using Furniture;

namespace FactoryMethod
{
    public class ClosetFactory : FurnitureFactory
    {
        private readonly FurnitureData _furnitureData = new(1, "closet", 2, 1, "storage");
        private readonly FurnitureDataFlags _furnitureDataFlags = new();

        public ClosetFactory(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            _furnitureData = furnitureData;
            _furnitureDataFlags = furnitureDataFlags;
        }

        public ClosetFactory()
        {

        }


        public override GeneralFurniture GetFurniture(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            Closet closet = new(furnitureData, furnitureDataFlags);
            return closet;
        }

        public override GeneralFurniture GetFurniture()
        {
            Closet closet = new(_furnitureData, _furnitureDataFlags);
            return closet;
        }

    }
}