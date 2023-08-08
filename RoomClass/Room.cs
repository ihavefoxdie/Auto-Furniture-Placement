using Furniture;
using Interfaces;

namespace Rooms
{
    //TODO: Integration with Zones class once (semi?)finished.
    //TODO: Should this class handle rotation and movement of IPolygon objects (i.e. GeneralFurniture)? Discuss.
    public sealed class Room : IPolygonContainer
    {
        #region General Properties
        public List<IPolygon> Polygons {
            get
            {
                List<IPolygon> _list = new();
                foreach(IPolygon polygon in FurnitureList) {
                    _list.Add(polygon);
                }
                return _list;
            }
        }
        
        public List<GeneralFurniture> FurnitureList { get; private set; }
        private List<GeneralFurniture> Doors { get; set; }
        private GeneralFurniture[]? Windows { get; set; }
        public int[,] RoomArray { get; set; }
        public int ContainerHeight { get; private set; }
        public int ContainerWidth { get; private set; }
        public double Penalty { get; set; }
        //List<Zones> ZonesList { get; private set; } //DEW EET
        public bool WindowsInRoom { get; private set; }
        #endregion


        #region Delegates
        public delegate void PathFinder(int[,] space, int entryCoordX, int entryCoordY, int desitnationX, int destinationY);
        public delegate bool CollisionDeterminer(decimal[,] vertices, decimal[] point);
        public CollisionDeterminer? DetermineCollision { get; set; }
        #endregion


        #region Constructor
        public Room(int height, int width, List<GeneralFurniture> doors, List<GeneralFurniture> items, bool windowed, GeneralFurniture[]? windows = null)
        {
            ContainerHeight = height;
            ContainerWidth = width;
            RoomArray = new int[width, height];
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

        /*public Rooms(int length, int width, Furniture[] doors, params Furniture[] items)
        {
            ContainerHeight = length;
            ContainerWidth = width;

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
        }*/
        #endregion


        //TODO Improve penalty evaluation by implementing a better method for more flexibility
        public void PenaltyEvaluation()
        {
            for (int i = 0; i < FurnitureList.Count; i++)
            {
                Penalty += OutOfBoundsDeterminer(FurnitureList[i]);

                if (FurnitureList[i].NearWall >= 0)
                    Penalty += NearWallPenalty(FurnitureList[i]);

                for (int k = 0; k < Doors.Count; k++)
                {
                    if (Collision(Doors[k], FurnitureList[i]))
                        Penalty += 10;
                }

                if (Windows is not null)
                {
                    if (!FurnitureList[i].IgnoreWindows)
                        for (int n = 0; n < Windows.GetLength(0); n++)
                        {
                            if (Collision(Windows[i], FurnitureList[i]))
                                Penalty += 10;
                        }
                }

                for (int j = i + 1; j < FurnitureList.Count; j++)
                {
                    if (Collision(FurnitureList[i], FurnitureList[j]))
                        Penalty += 10;
                }
            }
        }


        private int OutOfBoundsDeterminer(GeneralFurniture furniture)
        {
            int fine = 0;

            if (furniture.Center[1] < ContainerHeight && furniture.Center[0] < ContainerWidth &&
                furniture.Center[1] > 0 && furniture.Center[0] > 0)
            {
                for (int j = 0; j < furniture.Vertices.GetLength(0); j++)
                {
                    if (furniture.Vertices[j, 0] > ContainerWidth || furniture.Vertices[j, 1] > ContainerHeight)
                    {
                        fine += 10;
                        furniture.IsOutOfBounds = true;
                        break;
                    }
                    furniture.IsOutOfBounds = false;
                }
            }
            else
            {
                furniture.IsOutOfBounds = true;
                return fine += 10;
            }

            return fine;
        }

        public bool Collision(GeneralFurniture item1, GeneralFurniture item2)
        {
            if (DetermineCollision == null)
                return false;
            for (int i = 0; i < item2.Vertices.GetLength(0); i++)
            {
                decimal[] point = new decimal[] { item2.Vertices[i, 0], item2.Vertices[i, 1] };

                if (DetermineCollision(item1.Vertices, point))
                {
                    item2.IsCollided = true;
                    item1.IsCollided = true;
                    return true;
                }
            }

            item1.IsCollided = false;
            item2.IsCollided = false;
            return false;
        }

        private int NearWallPenalty(GeneralFurniture furniture)
        {
            int fine = 0;
            bool check = false;

            for (int i = 0; i < furniture.Vertices.GetLength(0); i++)
            {
                if (furniture.Vertices[i, 1] <= (0 + furniture.NearWall))
                {
                    check = true;
                    if (!(furniture.Rotation >= 260 && furniture.Rotation <= 280))
                        fine += 5;
                    break;
                }
                if (furniture.Vertices[i, 0] <= (0 + furniture.NearWall))
                {
                    check = true;
                    if (!((furniture.Rotation >= 350 && furniture.Rotation < 360) ||
                        (furniture.Rotation >= 0 && furniture.Rotation <= 10)))
                        fine += 5;
                    break;
                }
                if (furniture.Vertices[i, 1] >= (ContainerHeight - furniture.NearWall))
                {
                    check = true;
                    if (!(furniture.Rotation >= 80 && furniture.Rotation <= 100))
                        fine += 5;
                    break;
                }
                if (furniture.Vertices[i, 0] >= (ContainerWidth - furniture.NearWall))
                {
                    check = true;
                    if (!(furniture.Rotation >= 170 && furniture.Rotation < 190))
                        fine += 5;
                    break;
                }
            }

            if (!check)
            {
                fine += 10;
            }
            return fine;
        }
    }
}