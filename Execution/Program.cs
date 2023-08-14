using FactoryMethod;
using Furniture;
using Interfaces;
using Rasterization;
using RoomClass.Zones;
using Rooms;
using Vertex;
using Zones;

namespace Testing
{
    static class Program
    {
        static void Main()
        {
            //TODO Creating object have to use only FurnitureFactory
            FurnitureFactory furnitureFactory = new BedFactory();

            CupboardFactory cupboardFactory = new CupboardFactory();
            DresserFactory dresserFactory = new DresserFactory();

            PoufFactory poufFactory = new PoufFactory();
            SofaFactory sofaFactory = new SofaFactory();
            TableFactory tableFactory = new TableFactory();


            NightstandFactory nightstandFactory = new NightstandFactory();
            BedFactory bedFactory = new();

            ChairFactory chairFactory = new ChairFactory();
            DeskFactory deskFactory = new DeskFactory();

            DoorFactory doorFactory = new();

            int[] origRoomDim = new int[] { 160, 80 };
            int[] roomDim = new int[] { 160, 80 };

            List<GeneralFurniture> furnitures = new()
            {
                cupboardFactory.GetFurniture(),
                dresserFactory.GetFurniture(),
                poufFactory.GetFurniture(),
                sofaFactory.GetFurniture(),
                tableFactory.GetFurniture(),
                nightstandFactory.GetFurniture(),
                bedFactory.GetFurniture(),
                cupboardFactory.GetFurniture(),
                deskFactory.GetFurniture(),
            };
            /*for (int i = 0; i < furnitures.Count; i++)
            {
                furnitures[i].RotateVertex = VertexManipulator.VertexRotation;
            }
            furnitures[0].Move(0, 0);
            furnitures[1].Move(10, 15);*/
            //furnitures[0].Rotate(45);

            //Console.WriteLine(furnitures[0].Vertices[0, 0] + " " + furnitures[0].Vertices[0, 1]);
            //Console.WriteLine(furnitures[0].Vertices[1, 0] + " " + furnitures[0].Vertices[1, 1]);
            //Console.WriteLine(furnitures[0].Vertices[2, 0] + " " + furnitures[0].Vertices[2, 1]);
            //Console.WriteLine(furnitures[0].Vertices[3, 0] + " " + furnitures[0].Vertices[3, 1]);


            //Console.WriteLine(furnitures[0].Rotation);
            //int rotateFor = 360 - furnitures[0].Rotation;

            ////furnitures[0].Rotate(rotateFor);

            //Console.WriteLine(furnitures[0].Vertices[0, 0] + " " + furnitures[0].Vertices[0, 1]);
            //Console.WriteLine(furnitures[0].Vertices[1, 0] + " " + furnitures[0].Vertices[1, 1]);
            //Console.WriteLine(furnitures[0].Vertices[2, 0] + " " + furnitures[0].Vertices[2, 1]);
            //Console.WriteLine(furnitures[0].Vertices[3, 0] + " " + furnitures[0].Vertices[3, 1]);

            List<GeneralFurniture> door = new()
            {
                doorFactory.GetFurniture()
            };

            Room newRoom = new(40, 40, door, furnitures, _ = false, 1)
            {
                DetermineCollision = VertexManipulator.DetermineCollision
            };

            //var listToDelete = newRoom.InitializeZones();




            //Rasterizer.RasterizationMethod = LineDrawer.Line;

            ////newRoom.RasterizationMethod()

            ////newRoom.FurnitureList[0].Rotate(31);
            //newRoom.RoomArray = Rasterizer.Rasterization(newRoom.FurnitureList.ToList<IPolygon>(), newRoom.ContainerWidth, newRoom.ContainerWidth);


            //LineDrawer.Print(newRoom.RoomArray);






            //Console.WriteLine(newRoom.Collision(newRoom.FurnitureList[0], newRoom.FurnitureList[1]));
            //newRoom.PenaltyEvaluation();
            //Console.WriteLine(newRoom.FurnitureList[0].IsOutOfBounds); Console.WriteLine(newRoom.FurnitureList[1].IsOutOfBounds);
            //Console.WriteLine(newRoom.Penalty);


            #region AnnealingTesting

            newRoom.InitializeZones();
            List<AnnealingZone> annealingZones = new List<AnnealingZone>(newRoom.ZonesList.Count);
            SimulatedAnnealing simulatedAnnealing = new(newRoom.ZonesList, newRoom.Aisle, newRoom.ContainerWidth, newRoom.ContainerHeight, newRoom.Doors);



            #endregion



            Console.ReadLine();

        }
    }
}
