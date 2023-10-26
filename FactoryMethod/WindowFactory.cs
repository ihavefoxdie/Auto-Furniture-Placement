using Furniture;

namespace FactoryMethod
{
    public class WindowFactory : FurnitureFactory
    {
        private readonly FurnitureData _furnitureData = new(20, "window", 3, 30, "obstacles", 50, 0);
        private readonly FurnitureDataFlags _furnitureDataFlags = new();

        public WindowFactory(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            _furnitureData = furnitureData;
            _furnitureDataFlags = furnitureDataFlags;
        }

        public WindowFactory()
        {

        }


        public override Window GetFurniture(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            Window window = new(furnitureData, furnitureDataFlags);
            return window;
        }

        public override Window GetFurniture()
        {
            Window window = new(_furnitureData, _furnitureDataFlags);
            return window;
        }

    }
}