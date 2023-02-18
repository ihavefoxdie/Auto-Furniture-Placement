namespace RoomClass
{
    public class Room
    {
        public List<Furniture> FurnitureList { get; private set; }
        private Furniture[] Doors { get; set; }
        private Furniture[]? Windows { get; set; }
        public int RoomLength { get; private set; }
        public int RoomWidth { get; private set; }
        public double Fitness { get; private set; }
        //List<Zones> ZonesList { get; private set; } //DEW EET
        public bool WindowsInRoom { get; private set; }
        public Room(int length, int width, Furniture[] doors, Furniture[] windows, params Furniture[] items)
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
                throw new ArgumentNullException(nameof(doors), "The doors array is null!");
            Doors = doors;
            if (windows is null)
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

        public static bool Collision(Furniture item1, Furniture item2)
        {
            for (int i = 0; i < item2.Vertices.GetLength(0); i++)
            {
                double[] point = new double[] { item2.Vertices[i, 0], item2.Vertices[i, 1] };

                if (DetermineCollision(item1.Vertices, point))
                    return true;
            }

            return false;
        }

        private static bool DetermineCollision(double[,] vertices, double[] point)
        {
            bool collision = false;
            
            for (int i = 0, j = vertices.Length - 1; i < vertices.Length; j = i++)
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