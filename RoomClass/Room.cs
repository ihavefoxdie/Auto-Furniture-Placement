using Furniture;
using Interfaces;


namespace Rooms
{
    //TODO: Integration with Zones class once (semi?)finished.
    //TODO: Should this class handle rotation and movement of IPolygon objects (i.e. GeneralFurniture)? Discuss.
    //TODO: Clean this mess of a code!
    public sealed class Room : IPolygonGenesContainer
    {
        #region General Properties
        public List<IPolygon> Polygons
        {
            get
            {
                List<IPolygon> _list = new();
                foreach (IPolygon polygon in FurnitureArray)
                {
                    _list.Add(polygon);
                }
                return _list;
            }
        }

        private enum Wall
        {
            Left,
            Right,
            Up,
            Down
        }
        public GeneralFurniture[] FurnitureArray { get; private set; }
        public List<GeneralFurniture> Doors { get; set; }
        private List<GeneralFurniture>? Windows { get; set; }
        public int ContainerHeight { get; private set; }
        public int ContainerWidth { get; private set; }
        public double Penalty { get; set; }
        public List<IPolygon> ZonesList { get; set; } //DEW EET
        public bool WindowsInRoom { get; private set; }
        public int Aisle { get; private set; }
        #endregion


        #region Delegates
        //public delegate void PathFinder(int[,] space, int entryCoordX, int entryCoordY, int desitnationX, int destinationY);
        public delegate bool CollisionDeterminer(decimal[,] vertices, decimal[] point);
        public CollisionDeterminer? DetermineCollision { get; set; }

        public delegate void VertexRotation(ref decimal x, ref decimal y, double radians, int centerX, int centerY);
        public VertexRotation? RotateVertex { get; set; }
        #endregion


        #region Constructor

        public Room(int height, int width, List<GeneralFurniture> doors, List<GeneralFurniture> items, bool windowed, int aisle, List<GeneralFurniture>? windows = null)
        {
            Aisle = aisle;
            ContainerHeight = height;
            ContainerWidth = width;
            FurnitureArray = new GeneralFurniture[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is not null)
                    FurnitureArray[i] = items[i];
            }
            if (doors is null)
                throw new ArgumentNullException(nameof(doors), "The doors array is null!");
            Doors = doors;
            if (windows is null && windowed)
                throw new ArgumentNullException(nameof(windows), "The windows array is null!");
            Windows = windows;
            WindowsInRoom = windowed;
        }
        #endregion


        public void Randomize()
        {
            for (int i = 0; i < FurnitureArray.Length; i++)
            {
                for (int j = 0; j < 500; j++)
                {
                    Scatter(FurnitureArray[i]);
                    RandomRotation(FurnitureArray[i]);
                }
            }
        }

        public void Mutate()
        {
            List<GeneralFurniture> furnitureToMutate = new();

            int evenOrNot = new Random().Next(2);
            for (int i = 0; i < FurnitureArray.Length; i++)
            {
                if (i % 2 == evenOrNot)
                {
                    furnitureToMutate.Add(FurnitureArray[i]);
                }
            }
            bool scatter = false;
            bool randomRotation = false;
            bool wallAlignment = false;
            bool moveObjectToObject = false;
            bool alignment = false;
            bool moveToParent = false;
            Random selector = new();
            while (furnitureToMutate.Count > 0)
            {
                if (scatter && randomRotation && wallAlignment && moveObjectToObject && alignment && moveToParent)
                {
                    scatter = false;
                    randomRotation = false;
                    wallAlignment = false;
                    moveObjectToObject = false;
                    alignment = false;
                    moveToParent = false;
                }

                if (new Random().Next(3) == 0 && !scatter)
                {
                    scatter = true;
                    int value = selector.Next(furnitureToMutate.Count);
                    Scatter(furnitureToMutate.ElementAt(value));
                    furnitureToMutate.RemoveAt(value);
                    continue;
                }
                if (new Random().Next(2) != 0 && !randomRotation)
                {
                    randomRotation = true;
                    int value = selector.Next(furnitureToMutate.Count);
                    RandomRotation(furnitureToMutate.ElementAt(value));
                    furnitureToMutate.RemoveAt(value);
                    continue;
                }
                if (new Random().Next(5) == 0 && !wallAlignment)
                {
                    wallAlignment = true;
                    int value = selector.Next(furnitureToMutate.Count);
                    WallAlignment(furnitureToMutate.ElementAt(value));
                    furnitureToMutate.RemoveAt(value);
                    continue;
                }
                if (new Random().Next(3) == 0 && !moveObjectToObject)
                {
                    moveObjectToObject = true;
                    int value = selector.Next(furnitureToMutate.Count);
                    MoveObjectToObject(furnitureToMutate.ElementAt(value));
                    furnitureToMutate.RemoveAt(value);
                    continue;
                }
                if (new Random().Next(4) == 0 && !alignment)
                {
                    alignment = true;
                    int value = selector.Next(furnitureToMutate.Count);
                    Alighnment(furnitureToMutate.ElementAt(value));
                    furnitureToMutate.RemoveAt(value);
                    continue;
                }
                if (new Random().Next(4) == 0 && !moveToParent)
                {
                    moveToParent = true;
                    int value = selector.Next(furnitureToMutate.Count);
                    MoveToParent(furnitureToMutate.ElementAt(value));
                    furnitureToMutate.RemoveAt(value);
                    continue;
                }
            }
        }

        public IPolygonGenesContainer Crossover(IPolygonGenesContainer parent)
        {
            if (parent is not Room roomParent)
            {
                return parent;
            }
            Room child = new(this.ContainerHeight, this.ContainerWidth, new List<GeneralFurniture>(), new List<GeneralFurniture>(), this.WindowsInRoom, this.Aisle)
            {
                FurnitureArray = new GeneralFurniture[this.FurnitureArray.Length]
            };
            List<int> indexesToIgnore = new();

            int even = new Random().Next(2);

            for (int i = 0; i < this.FurnitureArray.Length; i++)
            {
                if (indexesToIgnore.Contains(i))
                    continue;
                if (i % 2 == even)
                {
                    child.FurnitureArray[i] = (GeneralFurniture)this.FurnitureArray[i].Clone();
                    if (FurnitureArray[i].Data.ParentIndex != null)
                    {
                        indexesToIgnore.Add((int)FurnitureArray[i].Data.ParentIndex!);
                        child.FurnitureArray[(int)FurnitureArray[i].Data.ParentIndex!] = FurnitureArray[(int)FurnitureArray[i].Data.ParentIndex!];
                    }
                    if (FurnitureArray[i].Data.ChildIndex != null)
                    {
                        indexesToIgnore.Add((int)FurnitureArray[i].Data.ChildIndex!);
                        child.FurnitureArray[(int)FurnitureArray[i].Data.ChildIndex!] = FurnitureArray[(int)FurnitureArray[i].Data.ChildIndex!];
                    }
                }
                else
                {
                    child.FurnitureArray[i] = (GeneralFurniture)roomParent.FurnitureArray[i].Clone();
                    if (roomParent.FurnitureArray[i].Data.ParentIndex != null)
                    {
                        indexesToIgnore.Add((int)roomParent.FurnitureArray[i].Data.ParentIndex!);
                        child.FurnitureArray[(int)roomParent.FurnitureArray[i].Data.ParentIndex!] = roomParent.FurnitureArray[(int)roomParent.FurnitureArray[i].Data.ParentIndex!];
                    }
                    if (roomParent.FurnitureArray[i].Data.ChildIndex != null)
                    {
                        indexesToIgnore.Add((int)roomParent.FurnitureArray[i].Data.ChildIndex!);
                        child.FurnitureArray[(int)roomParent.FurnitureArray[i].Data.ChildIndex!] = roomParent.FurnitureArray[(int)roomParent.FurnitureArray[i].Data.ChildIndex!];
                    }
                }
            }

            for (int i = 0; i < this.Doors.Count; i++)
            {
                child.Doors.Add((GeneralFurniture)this.Doors[i].Clone());
            }

            if (Windows != null)
            {
                child.Windows = new();
                for (int i = 0; i < this.Windows.Count; i++)
                {
                    child.Windows.Add((GeneralFurniture)this.Windows[i].Clone());
                }
            }

            child.DetermineCollision = this.DetermineCollision;
            child.RotateVertex = this.RotateVertex;
            child.PenaltyEvaluation();

            return child;
        }


        #region Mutation Types
        public void Scatter(GeneralFurniture item)
        {
            decimal x = new Random().Next(-1, 1);
            if (new Random().Next(5) >= 3)
                x *= item.Width / 2;
            if (new Random().Next(5) > 3)
                x += 3;

            decimal y = new Random().Next(-1, 1);
            if (new Random().Next(5) >= 3)
                y *= item.Height / 2;
            if (new Random().Next(5) > 3)
                y += 3;


            if (item.Data.ParentIndex != null)
            {
                if (SafeMove(FurnitureArray[(int)item.Data.ParentIndex], x, y) != 0)
                    return;
            }
            if (item.Data.ChildIndex != null)
            {
                if (SafeMove(FurnitureArray[(int)item.Data.ChildIndex], x, y) != 0)
                    return;
            }

            if (SafeMove(item, x, y) != 0) return;
        }

        public int RandomRotation(GeneralFurniture item)
        {
            int minusOrPlus = new Random().Next(2);
            int rotateFor;

            if (minusOrPlus > 0)
                rotateFor = 90;
            else
                rotateFor = -90;


            if (item.Data.ParentIndex != null)
            {
                return RandomRotation(FurnitureArray[(int)item.Data.ParentIndex]);
            }
            if (item.Data.ChildIndex != null)
            {
                if (SafeRotation(item, rotateFor) != 0)
                    return -1;
                return MoveToParent(FurnitureArray[(int)item.Data.ChildIndex]);
            }
            if (SafeRotation(item, rotateFor) != 0)
                return -1;

            return 0;
        }

        private Wall DetermineClosestWall(GeneralFurniture item)
        {
            bool left = false;
            bool right = false;
            bool up = false;
            bool down = false;

            decimal widthValue = item.Center[0] / ContainerWidth;
            decimal heightValue = item.Center[1] / ContainerHeight;

            if (widthValue > (decimal)0.5)
                right = true;
            else if (widthValue < (decimal)0.5)
                left = true;
            else
            {
                switch (new Random().Next(2))
                {
                    case 0:
                        left = true;
                        break;
                    case 1:
                        right = true;
                        break;
                }
            }

            if (heightValue > (decimal)0.5)
                down = true;
            else if (heightValue < (decimal)0.5)
                up = true;
            else
            {
                switch (new Random().Next(2))
                {
                    case 0:
                        up = true;
                        break;
                    case 1:
                        down = true;
                        break;
                }
            }


            if (widthValue < (decimal)0.5)
            {
                widthValue = 1 - widthValue;
            }
            if (heightValue < (decimal)0.5)
            {
                heightValue = 1 - heightValue;
            }


            if (widthValue < heightValue)
            {
                left = false; right = false;
            }
            else if (widthValue > heightValue)
            {
                up = false; down = false;
            }
            else
            {
                switch (new Random().Next(2))
                {
                    case 0:
                        left = false;
                        right = false;
                        break;
                    case 1:
                        up = false;
                        down = false;
                        break;
                }
            }

            if (left)
                return Wall.Left;
            if (right)
                return Wall.Right;
            if (up)
                return Wall.Up;
            if (down)
                return Wall.Down;

            return Wall.Down;
        }

        public void WallAlignment(GeneralFurniture item)
        {
            Wall direction = DetermineClosestWall(item);

            switch (direction)
            {
                case (Wall.Left):
                    if (SafeMove(item, -item.Center[0] + item.Width / 2, 0) != 0)
                        return;
                    break;

                case (Wall.Right):
                    if (SafeMove(item, ContainerWidth - item.Center[0] - item.Width / 2, 0) != 0)
                        return;
                    break;

                case (Wall.Up):
                    if (SafeMove(item, -item.Center[1] + item.Height / 2, 0) != 0)
                        return;
                    break;

                case (Wall.Down):
                    if (SafeMove(item, ContainerHeight - item.Center[1] - item.Height / 2, 0) != 0)
                        return;
                    break;
            }

            item.Data.ParentIndex = null;
            item.Data.ChildIndex = null;
        }

        public void MoveObjectToObject(GeneralFurniture item)
        {
            int index = 0;

            List<int> candidates = new();
            for (int i = 0; i < FurnitureArray.Length; i++)
            {
                if (FurnitureArray[i].ID != item.ID)
                {
                    candidates.Add(i);
                }
            }

            if (candidates.Count > 0)
            {
                index = candidates[new Random().Next(candidates.Count)];
            }

            decimal pointToMoveToX = Math.Round(FurnitureArray[index].Center[0] + (decimal)Math.Cos(FurnitureArray[index].Rotation) * FurnitureArray[index].Width);
            decimal pointToMoveToY = Math.Round(FurnitureArray[index].Center[1] + (decimal)Math.Sin(FurnitureArray[index].Rotation) * FurnitureArray[index].Width);

            decimal oldCenterX = item.Center[0];
            decimal oldCenterY = item.Center[1];

            if (SafeMove(item, (pointToMoveToX - oldCenterX), (pointToMoveToY - oldCenterY)) != 0)
                return;

            item.Data.ParentIndex = null;
            item.Data.ChildIndex = null;
        }

        //TODO: later in development introduce a whole new class for child-parent handling.
        public int MoveToParent(GeneralFurniture item)
        {
            if (item.Data.ParentID == -1)
                return -1;

            int index = -1;

            for (int i = 0; i < FurnitureArray.Length; i++)
            {
                if (FurnitureArray[i].ID == item.Data.ParentID)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                return -1;

            decimal pointToMoveToX = Math.Round(FurnitureArray[index].Center[0] + (decimal)Math.Cos(FurnitureArray[index].Rotation * Math.PI / 180) * FurnitureArray[index].Width, 3);
            decimal pointToMoveToY = Math.Round(FurnitureArray[index].Center[1] + (decimal)Math.Sin(FurnitureArray[index].Rotation * Math.PI / 180) * FurnitureArray[index].Width, 3);

            decimal oldCenterX = item.Center[0];
            decimal oldCenterY = item.Center[1];

            if (SafeMove(item, (pointToMoveToX - oldCenterX), (pointToMoveToY - oldCenterY)) != 0)
                return -1;

            if (SafeRotation(item, -item.Rotation + FurnitureArray[index].Rotation + 180) != 0)
            {
                Move(item, -(pointToMoveToX - oldCenterX), -(pointToMoveToY - oldCenterY));
                return -1;
            }

            item.Data.ParentIndex = index;
            FurnitureArray[index].Data.ChildIndex = FurnitureArray.ToList().IndexOf(item);

            return 0;
        }

        public void Alighnment(GeneralFurniture item)
        {
            GeneralFurniture alighnTo;
            if (FurnitureArray.GetLength(0) <= 1)
            {
                return;
            }
            while (true)
            {
                alighnTo = FurnitureArray[new Random().Next(FurnitureArray.Length)];

                if (alighnTo != item)
                    break;
            }

            int rotateTo = alighnTo.Rotation - item.Rotation;

            SafeRotation(item, rotateTo);
        }
        #endregion


        //TODO Improve penalty evaluation by implementing a better method for more flexibility
        #region Penalty Related
        public void PenaltyEvaluation()
        {
            Penalty = 0;
            for (int i = 0; i < FurnitureArray.Length; i++)
            {
                Penalty += OutOfBoundsDeterminer(FurnitureArray[i]);
                Penalty += NearWallPenalty(FurnitureArray[i]);

                for (int j = 0; j < FurnitureArray.Length; j++)
                {
                    if (FurnitureArray[j].ID == FurnitureArray[i].Data.ParentID && FurnitureArray[i].Data.ParentIndex == null)
                        Penalty += 10;
                }

                for (int k = 0; k < Doors.Count; k++)
                {
                    int doorCollision = (Collision(Doors[k], FurnitureArray[i]));
                    if (doorCollision != 0)
                        Penalty += 10 + doorCollision;
                }

                if (Windows is not null)
                {
                    if (!FurnitureArray[i].Flags.IgnoreWindows)
                        for (int n = 0; n < Windows.Count; n++)
                        {
                            int windowCollision = Collision(Windows[i], FurnitureArray[i]);
                            if (windowCollision != 0)
                                Penalty += 10 + windowCollision;
                        }
                }

                for (int j = i + 1; j < FurnitureArray.Length; j++)
                {
                    int furnitureCollision = Collision(FurnitureArray[i], FurnitureArray[j]);
                    if (furnitureCollision != 0)
                        Penalty += furnitureCollision;
                }
            }
        }

        private int OutOfBoundsDeterminer(GeneralFurniture furniture)
        {
            int fine = 0;

            for (int j = 0; j < furniture.Vertices.GetLength(0); j++)
            {
                if (Math.Round(furniture.Vertices[j, 0], 3) > (decimal)ContainerWidth || Math.Round(furniture.Vertices[j, 1], 3) > (decimal)ContainerHeight || Math.Round(furniture.Vertices[j, 0], 3) < 0 || Math.Round(furniture.Vertices[j, 1], 3) < 0)
                {
                    fine += 10;
                    furniture.IsOutOfBounds = true;
                    break;
                }
                furniture.IsOutOfBounds = false;
            }

            return fine;
        }

        public int Collision(GeneralFurniture item1, GeneralFurniture item2)
        {
            int penalty;
            penalty = ProcessCollision(item1, item2);
            if (item1.IsCollided)
            {
                return penalty;
            }
            penalty = ProcessCollision(item2, item1);
            if (item2.IsCollided)
            {
                return penalty;
            }
            return penalty;
        }

        private int ProcessCollision(GeneralFurniture item1, GeneralFurniture item2)
        {
            int penalty = 0;

            if (DetermineCollision == null)
                return penalty;

            bool collided = false;
            decimal[,] arrayOfVertices = new decimal[item1.Vertices.GetLength(0), 2];

            for (int j = 0; j < item1.Vertices.GetLength(0); j++)
            {
                for (int k = 0; k < item1.Vertices.GetLength(1); k++)
                {
                    arrayOfVertices[j, k] = Math.Round(item1.Vertices[j, k], 5);
                }
            }

            for (int i = 0; i < item2.Vertices.GetLength(0); i++)
            {
                decimal[] point = new decimal[] { Math.Round(item2.Vertices[i, 0], 5), Math.Round(item2.Vertices[i, 1], 5) };
                if (DetermineCollision(arrayOfVertices, point))
                {
                    item2.IsCollided = true;
                    item1.IsCollided = true;
                    collided = true;
                    penalty += 100;
                }
            }

            decimal[] center = new decimal[] { item2.Center[0], item2.Center[1] };
            if (DetermineCollision(arrayOfVertices, center))
            {
                item2.IsCollided = true;
                item1.IsCollided = true;
                collided = true;
                penalty += 150;
            }

            if (!collided)
            {
                item1.IsCollided = false;
                item2.IsCollided = false;
            }

            return penalty;
        }

        private double NearWallPenalty(GeneralFurniture item)
        {
            if (item.Flags.NearWall < 0)
                return 0;

            Wall direction = DetermineClosestWall(item);
            double fine = 0;
            double distance = 0;
            double distancePercentage = 0;

            switch (direction)
            {
                case Wall.Left:
                    if (item.Center[0] > item.Flags.NearWall)
                    {
                        distance = (double)item.Center[0];
                        distancePercentage = distance / (ContainerWidth / 2);
                    }
                    break;

                case Wall.Right:
                    if (item.Center[0] < ContainerWidth - item.Flags.NearWall)
                    {
                        distance = ContainerWidth - item.Flags.NearWall - (double)item.Center[0];
                        distancePercentage = distance / (ContainerWidth / 2);
                    }
                    break;

                case Wall.Up:
                    if (item.Center[1] > item.Flags.NearWall)
                    {
                        distance = (double)item.Center[1];
                        distancePercentage = distance / (ContainerHeight / 2);
                    }
                    break;

                case Wall.Down:
                    if (item.Center[0] < ContainerHeight - item.Flags.NearWall)
                    {
                        distance = ContainerHeight - item.Flags.NearWall - (double)item.Center[1];
                        distancePercentage = distance / (ContainerHeight / 2);
                    }
                    break;
            }

            if (distance != 0)
            {
                fine += distancePercentage * 10;
            }
            return fine;
        }
        #endregion


        #region Moving Furniture
        public void Move(GeneralFurniture item, decimal centerDeltaX, decimal centerDeltaY)
        {
            item.Center[0] += centerDeltaX;
            item.Center[1] += centerDeltaY;

            for (int i = 0; i < item.Vertices.GetLength(0); i++)
            {
                item.Vertices[i, 0] += centerDeltaX;
                item.Vertices[i, 1] += centerDeltaY;
            }
        }

        public int SafeMove(GeneralFurniture item, decimal x, decimal y)
        {
            Move(item, x, y);
            if (OutOfBoundsDeterminer(item) != 0)
            {
                Move(item, -x, -y);
                OutOfBoundsDeterminer(item);
                return 1;
            }
            for (int i = 0; i < FurnitureArray.Length; i++)
            {
                if (item != FurnitureArray[i])
                {
                    Collision(FurnitureArray[i], item);
                    if (item.IsCollided)
                    {
                        Move(item, -x, -y);
                        Collision(FurnitureArray[i], item);
                        return 1;
                    }
                }
            }
            return 0;
        }
        #endregion


        #region Rotating Furniture
        public void Rotate(GeneralFurniture item, int angle)
        {
            if (RotateVertex == null)
                return;

            item.ResetCoords();

            item.Rotation += angle;
            while (item.Rotation >= 360)
                item.Rotation -= 360;
            while (item.Rotation < 0)
                item.Rotation += 360;

            double radians = item.Rotation * (Math.PI / 180);

            for (int i = 0; i < item.Vertices.GetLength(0); i++)
            {
                RotateVertex(ref item.Vertices[i, 0], ref item.Vertices[i, 1], radians, (int)item.Center[0], (int)item.Center[1]);
                RotateVertex(ref item.ClearanceArea[i, 0], ref item.ClearanceArea[i, 1], radians, (int)item.Center[0], (int)item.Center[1]);
            }
        }

        public int SafeRotation(GeneralFurniture item, int rotateFor)
        {
            Rotate(item, rotateFor);
            for (int i = 0; i < item.Vertices.GetLength(0); i++)
            {
                item.Vertices[i, 0] = Math.Round(item.Vertices[i, 0], 4);
                item.Vertices[i, 1] = Math.Round(item.Vertices[i, 1], 4);
            }
            OutOfBoundsDeterminer(item);
            if (item.IsOutOfBounds)
            {
                Rotate(item, -rotateFor);
                OutOfBoundsDeterminer(item);
                return 1;
            }
            for (int i = 0; i < FurnitureArray.Length; i++)
            {
                if (item != FurnitureArray[i])
                {
                    Collision(item, FurnitureArray[i]);
                    if (item.IsCollided)
                    {
                        Rotate(item, -rotateFor);
                        Collision(item, FurnitureArray[i]); Collision(FurnitureArray[i], item);
                        return 1;
                    }
                }
            }
            return 0;
        }
        #endregion




        public object Clone()
        {
            List<GeneralFurniture> clonedDoors = new();
            List<GeneralFurniture> clonedFurniture = new();
            List<GeneralFurniture> clonedWindows = new();
            for (int i = 0; i < Doors.Count; i++)
            {
                clonedDoors.Add((GeneralFurniture)Doors[i].Clone());
            }

            for (int i = 0; i < FurnitureArray.Length; i++)
            {
                clonedFurniture.Add((GeneralFurniture)FurnitureArray[i].Clone());
            }
            if (Windows != null)
                for (int i = 0; i < Windows.Count; i++)
                {
                    clonedWindows.Add((GeneralFurniture)Windows[i].Clone());
                }

            Room clonedRoom = new(this.ContainerHeight, this.ContainerWidth, clonedDoors, clonedFurniture, WindowsInRoom, this.Aisle, clonedWindows)
            {
                RotateVertex = this.RotateVertex,
                DetermineCollision = this.DetermineCollision
            };
            return clonedRoom;
        }
    }
}