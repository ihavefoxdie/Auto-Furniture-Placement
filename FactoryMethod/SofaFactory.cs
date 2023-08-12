using Furniture;

namespace FactoryMethod
{
    public class SofaFactory : FurnitureFactory
    {
        private readonly FurnitureData _furnitureData = new(4, "sofa", 6, 2, "rest");
        private readonly FurnitureDataFlags _furnitureDataFlags = new();

        public SofaFactory(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            _furnitureData = furnitureData;
            _furnitureDataFlags = furnitureDataFlags;
        }

        public SofaFactory()
        {

        }


        public override GeneralFurniture GetFurniture(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            Sofa sofa = new(furnitureData, furnitureDataFlags);
            return sofa;
        }

        public override GeneralFurniture GetFurniture()
        {
            Sofa sofa = new(_furnitureData, _furnitureDataFlags);
            return sofa;
        }

    }
}