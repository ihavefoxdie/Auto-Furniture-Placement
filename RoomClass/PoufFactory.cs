using Furniture;

namespace FactoryMethod
{
    public class PoufFactory : FurnitureFactory
    {
        private readonly FurnitureData _furnitureData = new(3, "pouf", 1, 1, "rest");
        private readonly FurnitureDataFlags _furnitureDataFlags = new();

        public PoufFactory(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            _furnitureData = furnitureData;
            _furnitureDataFlags = furnitureDataFlags;
        }

        public PoufFactory()
        {

        }


        public override GeneralFurniture GetFurniture(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            Pouf pouf = new(furnitureData, furnitureDataFlags);
            return pouf;
        }

        public override GeneralFurniture GetFurniture()
        {
            Pouf pouf = new(_furnitureData, _furnitureDataFlags);
            return pouf;
        }

    }
}