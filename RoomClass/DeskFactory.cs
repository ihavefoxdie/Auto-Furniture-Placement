﻿using Furniture;

namespace FactoryMethod
{
    public class DeskFactory : FurnitureFactory
    {
        private readonly FurnitureData _furnitureData = new(10, "desk", 2, 1, "working");
        private readonly FurnitureDataFlags _furnitureDataFlags = new();

        public DeskFactory(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            _furnitureData = furnitureData;
            _furnitureDataFlags = furnitureDataFlags;
        }

        public DeskFactory()
        {

        }


        public override GeneralFurniture GetFurniture(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            Desk desk = new(furnitureData, furnitureDataFlags);
            return desk;
        }

        public override GeneralFurniture GetFurniture()
        {
            Desk desk = new(_furnitureData, _furnitureDataFlags);
            return desk;
        }

    }
}