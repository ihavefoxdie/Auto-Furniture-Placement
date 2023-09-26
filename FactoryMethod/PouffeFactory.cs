using Furniture;

namespace FactoryMethod
{
    public class PouffeFactory : FurnitureFactory
    {
        private readonly FurnitureData _furnitureData = new(3, "pouffe", 10, 10, "rest", 0, 0, 5);
        private readonly FurnitureDataFlags _furnitureDataFlags = new();

        public PouffeFactory(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            _furnitureData = furnitureData;
            _furnitureDataFlags = furnitureDataFlags;
        }

        public PouffeFactory()
        {

        }


        public override GeneralFurniture GetFurniture(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            Pouffe pouffe = new(furnitureData, furnitureDataFlags);
            return pouffe;
        }

        public override GeneralFurniture GetFurniture()
        {
            Pouffe pouffe = new(_furnitureData, _furnitureDataFlags);
            return pouffe;
        }

    }
}