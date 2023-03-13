namespace RoomClass
{
    static class Program
    {
        static void Main()
        {
            int[] origRoomDim = new int[] { 80, 80 };
            int[] roomDim = new int[] { 80, 80 };

            List<Furniture> furnitures = new()
            {
                new Furniture(1, "bed", 40, 32, "livingRoom", false),
                new Furniture(2, "table", 40, 40, "kitchen", false, 2)
            };

            furnitures[0].Move(-20, -16);
            furnitures[1].Move(-80, 80);
            //furnitures[0].Rotate(45);

            Console.WriteLine(furnitures[0].Vertices[0, 0] + " " + furnitures[0].Vertices[0, 1]);
            Console.WriteLine(furnitures[0].Vertices[1, 0] + " " + furnitures[0].Vertices[1, 1]);
            Console.WriteLine(furnitures[0].Vertices[2, 0] + " " + furnitures[0].Vertices[2, 1]);
            Console.WriteLine(furnitures[0].Vertices[3, 0] + " " + furnitures[0].Vertices[3, 1]);
            for (int i = 0; i < 1; i++)
                furnitures[0].Rotate(2);

            Console.WriteLine(furnitures[0].Rotation);
            int rotateFor = 360 - furnitures[0].Rotation;

            //furnitures[0].Rotate(rotateFor);

            Console.WriteLine(furnitures[0].Vertices[0, 0] + " " + furnitures[0].Vertices[0, 1]);
            Console.WriteLine(furnitures[0].Vertices[1, 0] + " " + furnitures[0].Vertices[1, 1]);
            Console.WriteLine(furnitures[0].Vertices[2, 0] + " " + furnitures[0].Vertices[2, 1]);
            Console.WriteLine(furnitures[0].Vertices[3, 0] + " " + furnitures[0].Vertices[3, 1]);

            int minimalVal = 0;
            for (int i = 0; i < furnitures.Count; i++)
            {
                for (int j = 0; j < furnitures[i].Vertices.GetLength(0); j++)
                {
                    for (int k = 0; k < furnitures[i].Vertices.GetLength(1); k++)
                    {
                        if ((int)furnitures[i].Vertices[j, k] < 0 && (int)furnitures[i].Vertices[j, k] < minimalVal)
                            minimalVal = (int)furnitures[i].Vertices[j, k];
                    }
                }
            }

            minimalVal = Math.Abs(minimalVal);



            int maxVal = 0;
            for (int i = 0; i < furnitures.Count; i++)
            {
                for (int j = 0; j < furnitures[i].Vertices.GetLength(0); j++)
                {
                    for (int k = 0; k < furnitures[i].Vertices.GetLength(1); k++)
                    {
                        if (((int)furnitures[i].Vertices[j, k] >= roomDim[0] || (int)furnitures[i].Vertices[j, k] >= roomDim[1]) && (int)furnitures[i].Vertices[j, k] > maxVal)
                            maxVal = (int)furnitures[i].Vertices[j, k] + 1;
                    }
                }
            }
            if (maxVal > roomDim[0])
                maxVal -= roomDim[0];
            else if (maxVal > roomDim[1])
                maxVal -= roomDim[1];
            int addVal = minimalVal + maxVal;
            roomDim[0] += addVal; roomDim[1] += addVal;
            maxVal = Math.Max(roomDim[0], roomDim[1]);
            roomDim[0] = maxVal; roomDim[1] = maxVal;

            int[,] space = new int[roomDim[0], roomDim[1]];
            for (int i = 0; i < furnitures.Count; i++)
            {
                for (int j = 0; j < furnitures[i].Vertices.GetLength(0); j++)
                {
                    if (j < furnitures[i].Vertices.GetLength(0) - 1)
                        Rasterization.Line((int)furnitures[i].Vertices[j, 0] + minimalVal, (int)furnitures[i].Vertices[j, 1] + minimalVal, (int)furnitures[i].Vertices[j + 1, 0] + minimalVal, (int)furnitures[i].Vertices[j + 1, 1] + minimalVal, space, furnitures[i].ID);
                    else
                        Rasterization.Line((int)furnitures[i].Vertices[j, 0] + minimalVal, (int)furnitures[i].Vertices[j, 1] + minimalVal, (int)furnitures[i].Vertices[0, 0] + minimalVal, (int)furnitures[i].Vertices[0, 1] + minimalVal, space, furnitures[i].ID);
                }
            }

            Rasterization.Line(minimalVal, minimalVal, origRoomDim[0] + minimalVal, minimalVal, space, -1);
            Rasterization.Line(origRoomDim[0] + minimalVal - 1, minimalVal, origRoomDim[0] + minimalVal -1, origRoomDim[1] + minimalVal - 1, space, -1);
            Rasterization.Line(origRoomDim[0] + minimalVal - 1, origRoomDim[1] + minimalVal - 1, minimalVal, origRoomDim[1] + minimalVal - 1, space, -1);
            Rasterization.Line(minimalVal, origRoomDim[1] + minimalVal - 1, minimalVal, minimalVal, space, -1);




            Rasterization.Print(space);

            Furniture[] door = new Furniture[] { new(0, "door", 2, 1, "ROOM", false, 0) };

            door[0].Move(20, 0);

            Room newRoom = new(40, 40, door, furnitures, _ = false);

            newRoom.FurnitureList[0].Rotate(31);

            Console.WriteLine(Room.Collision(newRoom.FurnitureList[0], newRoom.FurnitureList[1]));
            newRoom.PenaltyEvaluation();
            Console.WriteLine(newRoom.Penalty);
        }
    }
}
