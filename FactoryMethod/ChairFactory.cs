using Furniture;

namespace FactoryMethod
{
    public class ChairFactory : FurnitureFactory
    {
        private readonly FurnitureData _furnitureData = new(5, "chair", 1, 1, "working");
        private readonly FurnitureDataFlags _furnitureDataFlags = new();

        public ChairFactory(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            _furnitureData = furnitureData;
            _furnitureDataFlags = furnitureDataFlags;
        }

        public ChairFactory()
        {

        }


        public override GeneralFurniture GetFurniture(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            Chair chair = new(furnitureData, furnitureDataFlags);
            return chair;
        }

        public override GeneralFurniture GetFurniture()
        {
            Chair chair = new(_furnitureData, _furnitureDataFlags);
            return chair;
        }

    }
}