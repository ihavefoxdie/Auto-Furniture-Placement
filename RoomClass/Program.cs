namespace RoomClass
{
    static class Program
    {
        static void Main()
        {
            List<Furniture> furnitures = new();

            furnitures.Add(new Furniture(1, 2, 4, "livingRoom"));
            furnitures.Add(new Furniture(2, 2, 2, "kitchen"));
            furnitures[0].Move(2, 0);
            furnitures[0].Rotate(45);

            Furniture[] door = new Furniture[] { new(0, 2, 1, "ROOM") };

            door[0].Move(20, 0);

            Room newRoom = new(40, 40, door, furnitures, _ = false);

            Console.WriteLine(newRoom.Collision(newRoom.FurnitureList[0], newRoom.FurnitureList[1]));
        }
    }
}