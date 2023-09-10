using Execution;
using FactoryMethod;
using Furniture;

using GeneticAlgorithm;
using Interfaces;
using Rooms;
using System.Text.Json;
using Vertex;

namespace Testing;

static class Program
{
    static void sex(Room room)
    {
        List<PolygonForJson> rectangles = new List<PolygonForJson>();

        decimal[] center = new decimal[2];
        center[0] = room.ContainerWidth/2; center[1] = room.ContainerHeight/2;
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

        string serializedPolygon = File.ReadAllText("visualization\\rectangles.json");
    }
    static void Main()
    {
        TestingClass testingClass = new TestingClass();


        ////Process.Start("visualization\\testing shapes.exe");

        //BedFactory bedFactory = new();
        //TableFactory tableFactory = new();
        //DoorFactory doorFactory = new();
        //ChairFactory chairFactory = new();
        //PouffeFactory poufFactory = new();
        //ArmchairFactory armchairFactory = new();
        //CupboardFactory cupboardFactory = new();
        //DeskFactory deskFactory = new();


        //List<GeneralFurniture> furnitures = new();
        //furnitures.Add(bedFactory.GetFurniture());
        //furnitures.Add(tableFactory.GetFurniture());
        //furnitures.Add(tableFactory.GetFurniture());
        //furnitures.Add(chairFactory.GetFurniture());
        //furnitures.Add(poufFactory.GetFurniture());
        //furnitures.Add(armchairFactory.GetFurniture());
        //furnitures.Add(cupboardFactory.GetFurniture());
        //furnitures.Add(deskFactory.GetFurniture());

        //Room testingRoom = new(14, 14, new List<GeneralFurniture>(), furnitures, false, 0);
        //testingRoom.RotateVertex = VertexManipulator.VertexRotation;
        //testingRoom.DetermineCollision = VertexManipulator.DetermineCollision;

        //for (int i = 0; i < testingRoom.FurnitureArray.Length; i++)
        //{
        //    testingRoom.Move(testingRoom.FurnitureArray[i], testingRoom.ContainerWidth / 2, testingRoom.ContainerHeight / 2);
        //}

        //GeneticAlgoritm algo = new(testingRoom);





        ////algo.Start();
        //for (int i = 0; i < 100000000; i++)
        //{
        //    Thread.Sleep(100);
        //    testingRoom.Mutate();
        //    sex(testingRoom);
        //}

        /*for (int i = 0; i < 100000; i++)
        {
            testingRoom.Mutate();
            if (i % 10 == 0)
                Console.WriteLine(i);
        }*/

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
