using Execution;
using FactoryMethod;
using Furniture;
using Interfaces;
using Rooms;
using GeneticAlgorithm;
using System.Text.Json;
using Vertex;

namespace Testing;

static class Program
{
    static void Main()
    {
        BedFactory bedFactory = new();
        TableFactory tableFactory = new();
        DoorFactory doorFactory = new();
        ChairFactory chairFactory= new();

        List<PolygonForJson> rectangles = new List<PolygonForJson>();

        List<GeneralFurniture> furnitures = new();
        furnitures.Add(bedFactory.GetFurniture());
        furnitures.Add(tableFactory.GetFurniture());
        furnitures.Add(tableFactory.GetFurniture());
        furnitures.Add(chairFactory.GetFurniture());

        Room testingRoom = new(30, 30, new List<GeneralFurniture>(), furnitures, false);
        testingRoom.RotateVertex = VertexManipulator.VertexRotation;
        testingRoom.DetermineCollision = VertexManipulator.DetermineCollision;
        GeneticAlgoritm algo = new(testingRoom);

        for (int i = 0; i < testingRoom.FurnitureArray.Length; i++)
        {
            testingRoom.Move(testingRoom.FurnitureArray[i], testingRoom.ContainerWidth/2, testingRoom.ContainerHeight/2);
        }
        //algo.Start();
        /*for (int i = 0; i < 100; i++)
        {
            testingRoom.Mutate();
        }*/

        /*for (int i = 0; i < 1000000; i++)
        {
            for (int j = 0; j < testingRoom.FurnitureArray.Length; j++)
            {
                testingRoom.Scatter(testingRoom.FurnitureArray[j]);

            }
            for (int j = 0; j < testingRoom.FurnitureArray.Length; j++)
            {
                testingRoom.RandomRotation(testingRoom.FurnitureArray[j]);
                if (i % 10 == 0)
                {
                    testingRoom.WallAlignment(testingRoom.FurnitureArray[j]);
                }
            }

        }*/

        for (int i = 0; i < 100000; i++)
        {
            testingRoom.Mutate();
            if (i % 10 == 0)
                Console.WriteLine(i);
        }

        decimal[] center = new decimal[2];
        center[0] = 15; center[1] = 15;
        decimal[][] edges = new decimal[4][];
        for (int i = 0; i < edges.Length; i++)
            edges[i] = new decimal[2];

        edges[0][0] = 0; edges[0][1] = 0;
        edges[1][0] = 30; edges[1][1] = 0;
        edges[2][0] = 30; edges[2][1] = 30;
        edges[3][0] = 0; edges[3][1] = 30;

        rectangles.Add(new PolygonForJson(1213, 30, 30, center, edges, "Room"));
        foreach (IPolygon polygon in testingRoom.Polygons)
            rectangles.Add(new PolygonForJson(polygon));

        string jsonFile = JsonSerializer.Serialize(rectangles);
        File.WriteAllText("visualization\\rectangles.json", jsonFile);

        string serializedPolygon = File.ReadAllText("visualization\\rectangles.json");

        /*Thread.Sleep(3000);
        Process.Start("visualization\\testing shapes.exe");*/




        /*int[] origRoomDim = new int[] { 160, 80 };
        int[] roomDim = new int[] { 160, 80 };

        List<GeneralFurniture> furnitures = new()
        {
            bedFactory.GetFurniture(),
            tableFactory.GetFurniture()

            //new GeneralFurniture(1, "bed", 40, 32, "livingRoom", false),
            //new GeneralFurniture(2, "table", 40, 40, "kitchen", false, 2)
        };
        *//*for (int i = 0; i < furnitures.Count; i++)
        {
            furnitures[i].RotateVertex = VertexManipulator.VertexRotation;
        }
        furnitures[0].Move(0, 0);
        furnitures[1].Move(10, 15);*//*
        //furnitures[0].Rotate(45);

        Console.WriteLine(furnitures[0].Vertices[0, 0] + " " + furnitures[0].Vertices[0, 1]);
        Console.WriteLine(furnitures[0].Vertices[1, 0] + " " + furnitures[0].Vertices[1, 1]);
        Console.WriteLine(furnitures[0].Vertices[2, 0] + " " + furnitures[0].Vertices[2, 1]);
        Console.WriteLine(furnitures[0].Vertices[3, 0] + " " + furnitures[0].Vertices[3, 1]);
        

        Console.WriteLine(furnitures[0].Rotation);
        int rotateFor = 360 - furnitures[0].Rotation;

        //furnitures[0].Rotate(rotateFor);

        Console.WriteLine(furnitures[0].Vertices[0, 0] + " " + furnitures[0].Vertices[0, 1]);
        Console.WriteLine(furnitures[0].Vertices[1, 0] + " " + furnitures[0].Vertices[1, 1]);
        Console.WriteLine(furnitures[0].Vertices[2, 0] + " " + furnitures[0].Vertices[2, 1]);
        Console.WriteLine(furnitures[0].Vertices[3, 0] + " " + furnitures[0].Vertices[3, 1]);

        List<GeneralFurniture> door = new()
        {
            doorFactory.GetFurniture()
            //new(-1, "door", 15, 5, "ROOM", false, 0)
        };
        //door[0].Move(20, 0);

        Room newRoom = new(40, 40, door, furnitures, _ = false)
        {
            DetermineCollision = VertexManipulator.DetermineCollision
        };


        Rasterizer.RasterizationMethod = LineDrawer.Line;

        //newRoom.RasterizationMethod()

        //newRoom.FurnitureArray[0].Rotate(31);
        newRoom.RoomArray = Rasterizer.Rasterization(newRoom.FurnitureArray.ToList<IPolygon>(), newRoom.ContainerWidth, newRoom.ContainerWidth);

        
        LineDrawer.Print(newRoom.RoomArray);






        Console.WriteLine(newRoom.Collision(newRoom.FurnitureArray[0], newRoom.FurnitureArray[1]));
        newRoom.PenaltyEvaluation();
        Console.WriteLine(newRoom.FurnitureArray[0].IsOutOfBounds); Console.WriteLine(newRoom.FurnitureArray[1].IsOutOfBounds);
        Console.WriteLine(newRoom.Penalty);*/
    }
}
