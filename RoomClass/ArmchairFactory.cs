﻿using Furniture;

namespace FactoryMethod
{
    public class ArmchairFactory : FurnitureFactory
    {
        private readonly FurnitureData _furnitureData = new(9, "armchair", 1, 1, "working");
        private readonly FurnitureDataFlags _furnitureDataFlags = new();

        public ArmchairFactory(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            _furnitureData = furnitureData;
            _furnitureDataFlags = furnitureDataFlags;
        }

        public ArmchairFactory()
        {

        }


        public override GeneralFurniture GetFurniture(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            Armchair armchair = new(furnitureData, furnitureDataFlags);
            return armchair;
        }

        public override GeneralFurniture GetFurniture()
        {
            Armchair armchair = new(_furnitureData, _furnitureDataFlags);
            return armchair;
        }

    }
}