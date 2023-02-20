namespace RoomClass
{
    public class Room
    {
        public List<Furniture> FurnitureList { get; private set; }
        private Furniture[] Doors { get; set; }
        private Furniture[]? Windows { get; set; }
        public int RoomLength { get; private set; }
        public int RoomWidth { get; private set; }
        public double Penalty { get; private set; }
        //List<Zones> ZonesList { get; private set; } //DEW EET
        public bool WindowsInRoom { get; private set; }
        public Room(int length, int width, Furniture[] doors, List<Furniture> items, bool windowed, Furniture[]? windows = null)
        {
            RoomLength = length;
            RoomWidth = width;

            FurnitureList = new();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is not null)
                    FurnitureList.Add(items[i]);
            }

            if (doors is null)
                throw new ArgumentNullException(nameof(doors), "The doors array is null!");
            Doors = doors;
            if (windows is null && windowed)
                throw new ArgumentNullException(nameof(windows), "The windows array is null!");
            Windows = windows;
            WindowsInRoom = true;


            if (FurnitureList.Count == 0)
                throw new Exception("The room has no furniture!");

        }

        public Room(int length, int width, Furniture[] doors, params Furniture[] items)
        {
            RoomLength = length;
            RoomWidth = width;

            FurnitureList = new();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] is not null)
                    FurnitureList.Add(items[i]);
            }

            if (doors is null)
                throw new ArgumentNullException(nameof(doors), "The door array is null!");
            Doors = doors;
            WindowsInRoom = false;

            if (FurnitureList.Count == 0)
                throw new Exception("The room has no furniture!");
        }

        public void PenaltyEvaluation()
        {
            for (int i = 0; i < FurnitureList.Count; i++)
            {
                Penalty += OutOfBoundsDeterminer(FurnitureList[i]);

                for (int j = i + 1; j < FurnitureList.Count; j++)
                {
                    if (Collision(FurnitureList[i], FurnitureList[j]))
                        Penalty += 10;
                }
            }
        }

        private int OutOfBoundsDeterminer(Furniture furniture)
        {
            int Fine = 0;

            if (furniture.Center[1] < RoomLength && furniture.Center[0] < RoomWidth &&
                furniture.Center[1] > 0 && furniture.Center[0] > 0)
            {
                for (int j = 0; j < furniture.Vertices.GetLength(0); j++)
                {
                    if (furniture.Vertices[j, 0] > RoomWidth || furniture.Vertices[j, 1] > RoomLength)
                    {
                        Fine += 10;
                        furniture.IsOutOfBounds = true;
                        break;
                    }
                    furniture.IsOutOfBounds = false;
                }
            }
            else
            {
                furniture.IsOutOfBounds = true;
                return Fine += 10;
            }

            return Fine;
        }

        public static bool Collision(Furniture item1, Furniture item2)
        {
            for (int i = 0; i < item2.Vertices.GetLength(0); i++)
            {
                double[] point = new double[] { item2.Vertices[i, 0], item2.Vertices[i, 1] };

                if (DetermineCollision(item1.Vertices, point))
                {
                    item1.IsCollided = true;
                    return true;
                }
            }
            item1.IsCollided = false;
            return false;
        }

        private static bool DetermineCollision(double[,] vertices, double[] point)
        {
            bool collision = false;

            for (int i = 0, j = vertices.GetLength(0) - 1; i < vertices.GetLength(0); j = i++)
            {
                if (((vertices[i, 1] <= point[1] && point[1] < vertices[j, 1]) ||
                    (vertices[j, 1] <= point[1] && point[1] < vertices[i, 1])) &&
                    point[0] < (vertices[j, 0] - vertices[i, 0]) * (point[1] - vertices[i, 1]) /
                    (vertices[j, 1] - vertices[i, 1]) + vertices[i, 0])
                    collision = !collision;
            }
            return collision;
        }
    }
}