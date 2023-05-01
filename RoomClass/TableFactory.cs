using Furniture;

namespace FactoryMethod
{
    public class TableFactory : FurnitureFactory
    {
        private readonly FurnitureData _furnitureData = new(6, "table", 4, 4, "food");
        private readonly FurnitureDataFlags _furnitureDataFlags = new();

        public TableFactory(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            _furnitureData = furnitureData;
            _furnitureDataFlags = furnitureDataFlags;
        }

        public TableFactory()
        {

        }


        public override GeneralFurniture GetFurniture(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            Table table = new(furnitureData, furnitureDataFlags);
            return table;
        }

        public override GeneralFurniture GetFurniture()
        {
            Table table = new(_furnitureData, _furnitureDataFlags);
            return table;
        }

    }
}