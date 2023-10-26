using Furniture;

namespace FactoryMethod
{
    public class NightstandFactory : FurnitureFactory
    {
        private readonly FurnitureData _furnitureData = new(6, "nightstand", 10, 20, "bed", 20 ,20 , 7);
        private readonly FurnitureDataFlags _furnitureDataFlags = new();

        public NightstandFactory(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            _furnitureData = furnitureData;
            _furnitureDataFlags = furnitureDataFlags;
        }

        public NightstandFactory()
        {

        }


        public override GeneralFurniture GetFurniture(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            Nightstand nightstand = new(furnitureData, furnitureDataFlags);
            return nightstand;
        }

        public override GeneralFurniture GetFurniture()
        {
            Nightstand nightstand = new(_furnitureData, _furnitureDataFlags);
            return nightstand;
        }

    }
}