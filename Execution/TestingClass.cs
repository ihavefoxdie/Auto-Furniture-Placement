﻿using FactoryMethod;
using Furniture;
using GeneticAlgorithm;
using Interfaces;
using RoomClass.Zones;
using Rooms;
using System.Text.Json;
using Vertex;

namespace Execution
{
    public class TestingClass
    {
        //private FurnitureFactory? FurnitureFactory { get; set; }
        private Room? Room { get; set; }

        public void AnnealingTesting()
        {
            BedFactory bedFactory = new();
            TableFactory tableFactory = new();
            DoorFactory doorFactory = new();
            ChairFactory chairFactory = new();
            PouffeFactory poufFactory = new();
            ArmchairFactory armchairFactory = new();
            CupboardFactory cupboardFactory = new();
            DeskFactory deskFactory = new();

            List<GeneralFurniture> doors = new()
            {
                doorFactory.GetFurniture()
            };

            List<GeneralFurniture> furnitures = new()
            {
                bedFactory.GetFurniture(),
                tableFactory.GetFurniture(),
                tableFactory.GetFurniture(),
                chairFactory.GetFurniture(),
                poufFactory.GetFurniture(),
                armchairFactory.GetFurniture(),
                cupboardFactory.GetFurniture(),
                deskFactory.GetFurniture()
            };

            Room = new(20, 20, doors, furnitures, false, 1)
            {
                RotateVertex = VertexManipulator.VertexRotation,
                DetermineCollision = VertexManipulator.DetermineCollision
            };

            SimulatedAnnealing simulatedAnnealing = new(Room);
            Room.ZonesList = simulatedAnnealing.Launch().Zones.ToList<IPolygon>();
            PolySerializeFuckingZones(Room);
        }

        static string PolySerialize(Room room)
        {
            List<PolygonForJson> rectangles = new();

            decimal[] center = new decimal[2];
            center[0] = room.ContainerWidth / 2; center[1] = room.ContainerHeight / 2;
            decimal[][] edges = new decimal[4][];
            for (int i = 0; i < edges.Length; i++)
                edges[i] = new decimal[2];

            edges[0][0] = 0; edges[0][1] = 0;
            edges[1][0] = room.ContainerWidth; edges[1][1] = 0;
            edges[2][0] = room.ContainerWidth; edges[2][1] = room.ContainerHeight;
            edges[3][0] = 0; edges[3][1] = room.ContainerHeight;

            rectangles.Add(new PolygonForJson(1213, room.ContainerWidth, room.ContainerHeight, center, edges, ""));
            IPolygonGenesContainer contain = room;
            foreach (IPolygon polygon in contain.Polygons)
                rectangles.Add(new PolygonForJson(polygon));

            string jsonFile = JsonSerializer.Serialize(rectangles);
            try
            {
                File.WriteAllText("visualization\\rectangles.json", jsonFile);
            }
            catch
            { }

            return File.ReadAllText("visualization\\rectangles.json");
        }

        static string PolySerializeFuckingZones(Room room) //language!!!!
        {
            List<PolygonForJson> rectangles = new();

            decimal[] center = new decimal[2];
            center[0] = room.ContainerWidth / 2; center[1] = room.ContainerHeight / 2;
            decimal[][] edges = new decimal[4][];
            for (int i = 0; i < edges.Length; i++)
                edges[i] = new decimal[2];

            edges[0][0] = 0; edges[0][1] = 0;
            edges[1][0] = room.ContainerWidth; edges[1][1] = 0;
            edges[2][0] = room.ContainerWidth; edges[2][1] = room.ContainerHeight;
            edges[3][0] = 0; edges[3][1] = room.ContainerHeight;

            rectangles.Add(new PolygonForJson(1213, room.ContainerWidth, room.ContainerHeight, center, edges, ""));
            

            foreach (IPolygon polygon in room.ZonesList)
                rectangles.Add(new PolygonForJson(polygon));

            string jsonFile = JsonSerializer.Serialize(rectangles);
            try
            {
                File.WriteAllText("visualization\\rectangles.json", jsonFile);
            }
            catch
            { }

            return File.ReadAllText("visualization\\rectangles.json");
        }


        public void GeneticTesting()
        {
            BedFactory bedFactory = new();
            TableFactory tableFactory = new();
            DoorFactory doorFactory = new();
            ChairFactory chairFactory = new();
            PouffeFactory poufFactory = new();
            ArmchairFactory armchairFactory = new();
            CupboardFactory cupboardFactory = new();
            DeskFactory deskFactory = new();
            NightstandFactory nightStand = new();


            List<GeneralFurniture> furnitures = new()
            {
                bedFactory.GetFurniture(),
                nightStand.GetFurniture(),
                tableFactory.GetFurniture(),
                tableFactory.GetFurniture(),
                chairFactory.GetFurniture(),
                armchairFactory.GetFurniture(),
                cupboardFactory.GetFurniture(),
                deskFactory.GetFurniture()
            };

            Room = new(140, 140, new List<GeneralFurniture>(), furnitures, false, 0)
            {
                RotateVertex = VertexManipulator.VertexRotation,
                DetermineCollision = VertexManipulator.DetermineCollision
            };

            for (int i = 0; i < Room.FurnitureArray.Length; i++)
            {
                PolySerialize(Room);
                Room.Move(Room.FurnitureArray[i], Room.ContainerWidth / 2, Room.ContainerHeight / 2);
                PolySerialize(Room);
            }

            GeneticAlgoritm algo = new(Room);





            algo.Start();

            /*for (int i = 0; i < 100000000; i++)
            {
                Thread.Sleep(50);
                for (int j = 0; j < Room.FurnitureArray.Length; j++)
                {
                    if (Room.FurnitureArray[j].Data.ParentIndex != -1)
                        Console.WriteLine(Room.FurnitureArray[j].Name + " has a parent at " +
                            Room.FurnitureArray[j].Data.ParentIndex);
                }
                Room.Mutate();
                PolySerialize(Room);
            }*/
        }
    }
}
