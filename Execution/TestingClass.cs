using FactoryMethod;
using Furniture;
using GeneticAlgorithm;
using Interfaces;
using Rooms;
using System.Text.Json;
using Vertex;

namespace Execution
{
    public class TestingClass
    {
        private Room? Room { get; set; }


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
            WindowFactory window = new();

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

            List<GeneralFurniture> doors = new()
            {
                doorFactory.GetFurniture()
            };

            List<GeneralFurniture> windows = new()
            {
                window.GetFurniture()
            };

            Room = new(90, 90, doors, furnitures, windows)
            {
                RotateVertex = VertexManipulator.VertexRotation,
                DetermineCollision = VertexManipulator.DetermineCollision
            };

            Room.Move(Room.Doors[0], -Room.Doors[0].Center[0] + 20, -Room.Doors[0].Center[1]);
            Room.Rotate(Room.Doors[0], 90);
            Room.Move(Room.Windows![0], -Room.Windows![0].Center[0] + 25, -Room.Windows![0].Center[1] + Room.ContainerHeight);
            Room.Rotate(Room.Windows![0], 270);

            for (int i = 0; i < Room.FurnitureArray.Length; i++)
            {
                PolySerialize(Room);
                Room.Move(Room.FurnitureArray[i], Room.ContainerWidth / 2, Room.ContainerHeight / 2);
                PolySerialize(Room);
            }

            GeneticAlgoritm algo = new(Room);

            algo.Start();
        }
    }
}
