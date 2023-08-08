using Interfaces;
using Furniture;
using Rooms;
using Vertex;
using Rasterization;
using FactoryMethod;

namespace Testing
{
    static class Program
    {
        static void Main()
        {
            BedFactory bedFactory = new();
            TableFactory tableFactory = new();
            DoorFactory doorFactory = new();

            int[] origRoomDim = new int[] { 160, 80 };
            int[] roomDim = new int[] { 160, 80 };

            List<GeneralFurniture> furnitures = new()
            {
                bedFactory.GetFurniture(),
                tableFactory.GetFurniture()

                //new GeneralFurniture(1, "bed", 40, 32, "livingRoom", false),
                //new GeneralFurniture(2, "table", 40, 40, "kitchen", false, 2)
            };
            for (int i = 0; i < furnitures.Count; i++)
            {
                furnitures[i].RotateVertex = VertexManipulator.VertexRotation;
            }
            furnitures[0].Move(0, 0);
            furnitures[1].Move(10, 15);
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
            door[0].Move(20, 0);

            Room newRoom = new(40, 40, door, furnitures, _ = false)
            {
                DetermineCollision = VertexManipulator.DetermineCollision
            };


            Rasterizer.RasterizationMethod = LineDrawer.Line;

            //newRoom.RasterizationMethod()

            //newRoom.FurnitureList[0].Rotate(31);
            newRoom.RoomArray = Rasterizer.Rasterization(newRoom.FurnitureList.ToList<IPolygon>(), newRoom.ContainerWidth, newRoom.ContainerWidth);

            
            LineDrawer.Print(newRoom.RoomArray);






            Console.WriteLine(newRoom.Collision(newRoom.FurnitureList[0], newRoom.FurnitureList[1]));
            newRoom.PenaltyEvaluation();
            Console.WriteLine(newRoom.FurnitureList[0].IsOutOfBounds); Console.WriteLine(newRoom.FurnitureList[1].IsOutOfBounds);
            Console.WriteLine(newRoom.Penalty);
        }
    }
}
