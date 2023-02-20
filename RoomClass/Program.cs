namespace RoomClass
{
    static class Program
    {
        static void Main()
        {
            List<Furniture> furnitures = new()
            {
                new Furniture(1, 2, 4, "livingRoom"),
                new Furniture(2, 2, 2, "kitchen")
            };

            furnitures[0].Move(-1, 0);
            furnitures[0].Rotate(45);

            Furniture[] door = new Furniture[] { new(0, 2, 1, "ROOM") };

            door[0].Move(20, 0);

            Room newRoom = new(40, 40, door, furnitures, _ = false);

            Console.WriteLine(Room.Collision(newRoom.FurnitureList[0], newRoom.FurnitureList[1]));
            newRoom.PenaltyEvaluation();
            Console.WriteLine(newRoom.Penalty);
        }
    }
}