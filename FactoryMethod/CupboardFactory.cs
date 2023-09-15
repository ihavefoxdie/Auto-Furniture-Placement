﻿using Furniture;

namespace FactoryMethod
{
    public class CupboardFactory : FurnitureFactory
    {
        private readonly FurnitureData _furnitureData = new(0, "сupboard", 2, 1, "storage");
        private readonly FurnitureDataFlags _furnitureDataFlags = new();

        public CupboardFactory(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            _furnitureData = furnitureData;
            _furnitureDataFlags = furnitureDataFlags;
        }

        public CupboardFactory()
        {

        }


        public override GeneralFurniture GetFurniture(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            Cupboard cupboard = new(furnitureData, furnitureDataFlags);
            return cupboard;
        }

        public override GeneralFurniture GetFurniture()
        {
            Cupboard cupboard = new(_furnitureData, _furnitureDataFlags);
            return cupboard;
        }

    }
}