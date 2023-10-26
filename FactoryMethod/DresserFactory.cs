using Furniture;

namespace FactoryMethod
{
    public class DresserFactory : FurnitureFactory
    {
        private readonly FurnitureData _furnitureData = new(2, "dresser", 20, 30, "storage", 40);
        private readonly FurnitureDataFlags _furnitureDataFlags = new(false, 3);

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