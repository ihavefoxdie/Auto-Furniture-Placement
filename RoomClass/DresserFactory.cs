using Furniture;

namespace FactoryMethod
{
    public class DresserFactory : FurnitureFactory
    {
        private readonly FurnitureData _furnitureData = new(2, "dresser", 2, 1, "storage");
        private readonly FurnitureDataFlags _furnitureDataFlags = new();

        public DresserFactory(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            _furnitureData = furnitureData;
            _furnitureDataFlags = furnitureDataFlags;
        }

        public DresserFactory()
        {

        }


        public override GeneralFurniture GetFurniture(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            Dresser dresser = new(furnitureData, furnitureDataFlags);
            return dresser;
        }

        public override GeneralFurniture GetFurniture()
        {
            Dresser dresser = new(_furnitureData, _furnitureDataFlags);
            return dresser;
        }

    }
}