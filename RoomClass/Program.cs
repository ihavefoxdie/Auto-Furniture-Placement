namespace RoomClass
{
    static class Program
    {
        static void Main()
        {
            List<Furniture> furnitures = new()
            {
                new Furniture(1, 2, 4, "livingRoom", false),
                new Furniture(2, 2, 2, "kitchen", false, 2)
            };

            furnitures[0].Move(-1, 0);
            //furnitures[0].Rotate(45);

            Console.WriteLine(furnitures[0].Vertices[0, 0] + " " + furnitures[0].Vertices[0, 1]);
            Console.WriteLine(furnitures[0].Vertices[1, 0] + " " + furnitures[0].Vertices[1, 1]);
            Console.WriteLine(furnitures[0].Vertices[2, 0] + " " + furnitures[0].Vertices[2, 1]);
            Console.WriteLine(furnitures[0].Vertices[3, 0] + " " + furnitures[0].Vertices[3, 1]);
            for (int i = 0; i < 1; i++)
                furnitures[0].Rotate(1);

            Console.WriteLine(furnitures[0].Rotation);
            int rotateFor = 360 - furnitures[0].Rotation;

            furnitures[0].Rotate(rotateFor);

            Console.WriteLine(furnitures[0].Vertices[0, 0] + " " + furnitures[0].Vertices[0, 1]);
            Console.WriteLine(furnitures[0].Vertices[1, 0] + " " + furnitures[0].Vertices[1, 1]);
            Console.WriteLine(furnitures[0].Vertices[2, 0] + " " + furnitures[0].Vertices[2, 1]);
            Console.WriteLine(furnitures[0].Vertices[3, 0] + " " + furnitures[0].Vertices[3, 1]);

            Furniture[] door = new Furniture[] { new(0, 2, 1, "ROOM", false, 0) };

            door[0].Move(20, 0);

            Room newRoom = new(40, 40, door, furnitures, _ = false);

            newRoom.FurnitureList[0].Rotate(31);

            Console.WriteLine(Room.Collision(newRoom.FurnitureList[0], newRoom.FurnitureList[1]));
            newRoom.PenaltyEvaluation();
            Console.WriteLine(newRoom.Penalty);
        }
    }
}