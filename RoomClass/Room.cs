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
                foreach (IPolygon polygon in Doors)
                {
                    _list.Add(polygon);
                }
                if(Windows != null)
                {
                    foreach (IPolygon polygon in Windows)
                    {
                        _list.Add(polygon);
                    }
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
        public List<GeneralFurniture> Doors { get; private set; }
        public List<GeneralFurniture>? Windows { get; private set; }
        public int ContainerHeight { get; private set; }
        public int ContainerWidth { get; private set; }
        public double Penalty { get; set; }
        //public List<IPolygon> ZonesList { get; set; } //DEW EET
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

                for (int j = 0; j < new Random().Next(400, 5000); j++)
                {
                    Scatter(FurnitureArray[i]);
                    RandomRotation(FurnitureArray[i]);
                }
            }
        }

        public void Mutate()
        {
            List<GeneralFurniture> furnitureToMutate = new();
            int amountToMutate = FurnitureArray.Length / 2;
            if (amountToMutate == 0) amountToMutate = 1;

            while (true)
            {
                for (int i = 0; i < FurnitureArray.Length; i++)
                {
                    if (furnitureToMutate.Contains(FurnitureArray[i]))
                        continue;

                    int percent = new Random().Next(100);
                    if (percent > 49)
                    {
                        furnitureToMutate.Add(FurnitureArray[i]);
                    }

                    if (furnitureToMutate.Count == amountToMutate)
                        break;
                }
                if (furnitureToMutate.Count == amountToMutate)
                    break;
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

                if (new Random().Next(100) > 69 && !scatter)
                {
                    scatter = true;
                    int value = selector.Next(furnitureToMutate.Count);
                    Scatter(furnitureToMutate.ElementAt(value));
                    furnitureToMutate.RemoveAt(value);
                    continue;
                }
                if (new Random().Next(100) > 49 && !randomRotation)
                {
                    randomRotation = true;
                    int value = selector.Next(furnitureToMutate.Count);
                    RandomRotation(furnitureToMutate.ElementAt(value));
                    furnitureToMutate.RemoveAt(value);
                    continue;
                }
                if (new Random().Next(100) > 79 && !wallAlignment)
                {
                    wallAlignment = true;
                    int value = selector.Next(furnitureToMutate.Count);
                    WallAlignment(furnitureToMutate.ElementAt(value));
                    furnitureToMutate.RemoveAt(value);
                    continue;
                }
                if (new Random().Next(100) > 84 && !moveObjectToObject)
                {
                    moveObjectToObject = true;
                    int value = selector.Next(furnitureToMutate.Count);
                    MoveObjectToObject(furnitureToMutate.ElementAt(value));
                    furnitureToMutate.RemoveAt(value);
                    continue;
                }
                if (new Random().Next(100) > 74 && !alignment)
                {
                    alignment = true;
                    int value = selector.Next(furnitureToMutate.Count);
                    Alighnment(furnitureToMutate.ElementAt(value));
                    furnitureToMutate.RemoveAt(value);
                    continue;
                }
                if (new Random().Next(100) > 79 && !moveToParent)
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

            Room child = new(ContainerHeight, ContainerWidth, new List<GeneralFurniture>(), new List<GeneralFurniture>(), WindowsInRoom, Aisle)
            {
                FurnitureArray = new GeneralFurniture[this.FurnitureArray.Length]
            };

            List<int> indexesToIgnore = new();

            int even = new Random().Next(2);

            for (int i = 0; i < this.FurnitureArray.Length; i++)
            {
                if (indexesToIgnore.Contains(i))
                    continue;

                indexesToIgnore.Add(i);

                if (i % 2 == even)
                {
                    child.FurnitureArray[i] = (GeneralFurniture)FurnitureArray[i].Clone();

                    if (child.FurnitureArray[i].Data.ParentIndex != -1)
                    {
                        if (child.FurnitureArray[FurnitureArray[i].Data.ParentIndex] == null)
                        {
                            indexesToIgnore.Add(FurnitureArray[i].Data.ParentIndex);
                            child.FurnitureArray[FurnitureArray[i].Data.ParentIndex] = (GeneralFurniture)FurnitureArray[FurnitureArray[i].Data.ParentIndex].Clone();
                        }
                        else
                        {
                            child.FurnitureArray[i].Data.ParentIndex = -1;
                        }
                    }
                    if (child.FurnitureArray[i].Data.ChildIndex != -1)
                    {
                        if (child.FurnitureArray[FurnitureArray[i].Data.ChildIndex] == null)
                        {
                            indexesToIgnore.Add(FurnitureArray[i].Data.ChildIndex);
                            child.FurnitureArray[FurnitureArray[i].Data.ChildIndex] = (GeneralFurniture)FurnitureArray[FurnitureArray[i].Data.ChildIndex].Clone();
                        }
                        else
                        {
                            child.FurnitureArray[i].Data.ChildIndex = -1;
                        }
                    }
                }
                else
                {
                    child.FurnitureArray[i] = (GeneralFurniture)roomParent.FurnitureArray[i].Clone();

                    if (child.FurnitureArray[i].Data.ParentIndex != -1)
                    {
                        if (child.FurnitureArray[roomParent.FurnitureArray[i].Data.ParentIndex] == null)
                        {
                            indexesToIgnore.Add(roomParent.FurnitureArray[i].Data.ParentIndex);
                            child.FurnitureArray[roomParent.FurnitureArray[i].Data.ParentIndex] = (GeneralFurniture)roomParent.FurnitureArray[roomParent.FurnitureArray[i].Data.ParentIndex].Clone();
                        }
                        else
                        {
                            child.FurnitureArray[i].Data.ParentIndex = -1;
                        }
                    }
                    if (child.FurnitureArray[i].Data.ChildIndex != -1)
                    {
                        if (child.FurnitureArray[roomParent.FurnitureArray[i].Data.ChildIndex] == null)
                        {
                            indexesToIgnore.Add(roomParent.FurnitureArray[i].Data.ChildIndex);
                            child.FurnitureArray[roomParent.FurnitureArray[i].Data.ChildIndex] = (GeneralFurniture)roomParent.FurnitureArray[roomParent.FurnitureArray[i].Data.ChildIndex].Clone();
                        }
                        else
                        {
                            child.FurnitureArray[i].Data.ChildIndex = -1;
                        }
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
            decimal x = new Random().Next(-1, 2);
            decimal y = new Random().Next(-1, 2);

            if (!item.IsCollided && new Random().Next(100) > 80)
            {
                y *= 5;
                x *= 5;
            }
            else if (item.IsCollided && new Random().Next(100) < 80)
            {
                if (new Random().Next(100) > 40)
                    x *= item.Depth / 2;
                if (new Random().Next(100) > 60)
                    x += 30;
                if (new Random().Next(100) > 80)
                    x += 30;

                if (new Random().Next(100) > 40)
                    y *= item.FrontWidth / 2;
                if (new Random().Next(100) > 60)
                    y += 30;
                if (new Random().Next(100) > 80)
                    y += 30;
            }

            x = Math.Round(x, 5);
            y = Math.Round(y, 5);

            if (SafeMove(item, x, y) != 0)
                return;

            if (item.Data.ParentIndex != -1)
            {
                if (SafeMove(FurnitureArray[item.Data.ParentIndex], x, y) != 0)
                {
                    Move(item, -x, -y);
                    return;
                }
            }
            if (item.Data.ChildIndex != -1)
            {
                if (SafeMove(FurnitureArray[item.Data.ChildIndex], x, y) != 0)
                {
                    Move(item, -x, -y);
                    return;
                }
            }
        }

        public int RandomRotation(GeneralFurniture item)
        {
            int minusOrPlus = new Random().Next(2);
            int rotateFor;

            if (minusOrPlus > 0)
                rotateFor = 90;
            else
                rotateFor = -90;


            return ProcessRotation(item, rotateFor);
        }

        private int ProcessRotation(GeneralFurniture item, int rotateFor)
        {
            if (item.Data.ParentIndex != -1)
            {
                return ProcessRotation(FurnitureArray[item.Data.ParentIndex], rotateFor);
            }
            if (item.Data.ChildIndex != -1)
            {
                if (SafeRotation(item, rotateFor) != 0)
                    return -1;

                if (MoveToParent(FurnitureArray[item.Data.ChildIndex], FurnitureArray[item.Data.ChildIndex].Data.ParentIndex) != 0)
                {
                    SafeRotation(item, -rotateFor);
                    return -1;
                }

                return 0;
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
                    if (SafeMove(item, -item.Center[0] + item.Depth / 2, 0) != 0)
                        return;
                    break;

                case (Wall.Right):
                    if (SafeMove(item, ContainerWidth - item.Center[0] - item.Depth / 2, 0) != 0)
                        return;
                    break;

                case (Wall.Up):
                    if (SafeMove(item, -item.Center[1] + item.FrontWidth / 2, 0) != 0)
                        return;
                    break;

                case (Wall.Down):
                    if (SafeMove(item, ContainerHeight - item.Center[1] - item.FrontWidth / 2, 0) != 0)
                        return;
                    break;
            }

            if (item.Data.ParentIndex != -1)
            {
                FurnitureArray[item.Data.ParentIndex].Data.ChildIndex = -1;
            }
            if (item.Data.ChildIndex != -1)
            {
                FurnitureArray[item.Data.ChildIndex].Data.ParentIndex = -1;
            }

            item.Data.ParentIndex = -1;
            item.Data.ChildIndex = -1;
        }

        public void MoveObjectToObject(GeneralFurniture item)
        {
            List<int> candidates = new();
            for (int i = 0; i < FurnitureArray.Length; i++)
            {
                if (FurnitureArray[i].ID != item.ID)
                {
                    candidates.Add(i);
                }
            }

            int index;
            if (candidates.Count > 0)
            {
                index = candidates[new Random().Next(candidates.Count)];
            }
            else
                return;

            decimal pointToMoveToX = Math.Round(FurnitureArray[index].Center[0] + (decimal)Math.Cos(FurnitureArray[index].Rotation) * FurnitureArray[index].Depth);
            decimal pointToMoveToY = Math.Round(FurnitureArray[index].Center[1] + (decimal)Math.Sin(FurnitureArray[index].Rotation) * FurnitureArray[index].Depth);

            decimal oldCenterX = item.Center[0];
            decimal oldCenterY = item.Center[1];

            if (SafeMove(item, (pointToMoveToX - oldCenterX), (pointToMoveToY - oldCenterY)) != 0)
                return;

            if (item.Data.ParentIndex != -1)
            {
                FurnitureArray[item.Data.ParentIndex].Data.ChildIndex = -1;
            }

            if (item.Data.ChildIndex != -1)
            {
                FurnitureArray[item.Data.ChildIndex].Data.ParentIndex = -1;
            }

            item.Data.ParentIndex = -1;
            item.Data.ChildIndex = -1;
        }

        //TODO: later in development introduce a whole new class for child-parent handling.
        public int MoveToParent(GeneralFurniture item, int index = -1)
        {
            if (item.Data.ParentID == -1)
                return -1;

            if (index == -1)
            {
                for (int i = 0; i < FurnitureArray.Length; i++)
                {
                    if (FurnitureArray[i].ID == item.Data.ParentID && FurnitureArray[i].Data.ChildIndex == -1)
                    {
                        index = i;
                        break;
                    }
                }

                if (index == -1)
                    return -1;
            }

            decimal pointToMoveToX = Math.Round(FurnitureArray[index].Center[0] + (decimal)Math.Cos(FurnitureArray[index].Rotation * Math.PI / 180) * (FurnitureArray[index].Depth / 2 + item.Depth / 2 + 1), 3);
            decimal pointToMoveToY = Math.Round(FurnitureArray[index].Center[1] + (decimal)Math.Sin(FurnitureArray[index].Rotation * Math.PI / 180) * (FurnitureArray[index].Depth / 2 + item.Depth / 2 + 1), 3);

            decimal oldCenterX = item.Center[0];
            decimal oldCenterY = item.Center[1];

            if (SafeMove(item, (pointToMoveToX - oldCenterX), (pointToMoveToY - oldCenterY)) != 0)
                return -1;

            if (SafeRotation(item, -item.Rotation + FurnitureArray[index].Rotation + 180) != 0)
            {
                Move(item, -(pointToMoveToX - oldCenterX), -(pointToMoveToY - oldCenterY));
                return -1;
            }

            if (item.Data.ParentIndex != -1)
            {
                FurnitureArray[item.Data.ParentIndex].Data.ChildIndex = -1;
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

            ProcessRotation(item, rotateTo);
        }
        #endregion

        #region Penalty Related
        public void PenaltyEvaluation()
        {
            Penalty = 0;
            for (int i = 0; i < FurnitureArray.Length; i++)
            {
                Penalty += OutOfBoundsDeterminer(FurnitureArray[i]);
                Penalty += NearWallPenalty(FurnitureArray[i]);
                Penalty = Math.Round(Penalty, 5);

                for (int j = 0; j < FurnitureArray.Length; j++)
                {
                    if (FurnitureArray[j].ID == FurnitureArray[i].Data.ParentID && (FurnitureArray[i].Data.ParentIndex == -1 && FurnitureArray[j].Data.ChildIndex == -1))
                        Penalty += 10;
                }

                for (int k = 0; k < Doors.Count; k++)
                {
                    int doorCollision = Collision(Doors[k], FurnitureArray[i]);
                    int clearanceCollision = ClearenceAreaCollision(FurnitureArray[i], Doors[k]);
                    if (doorCollision != 0 || clearanceCollision != 0)
                        Penalty += 10 + doorCollision + clearanceCollision;
                }

                if (Windows is not null)
                {
                    if (!FurnitureArray[i].Flags.IgnoreWindows)
                        for (int n = 0; n < Windows.Count; n++)
                        {
                            int windowCollision = Collision(Windows[n], FurnitureArray[i]);
                            int clearanceCollision = ClearenceAreaCollision(FurnitureArray[i], Windows[n]);
                            if (windowCollision != 0 || clearanceCollision != 0)
                                Penalty += 10 + windowCollision + clearanceCollision;
                        }
                }

                for (int j = i + 1; j < FurnitureArray.Length; j++)
                {
                    int furnitureCollision = Collision(FurnitureArray[i], FurnitureArray[j]);
                    Penalty += furnitureCollision;

                    if (FurnitureArray[i].Data.ExtraWidth != 0 || FurnitureArray[i].Data.ExtraDepth != 0)
                    {
                        if (FurnitureArray[i].Data.ParentID != FurnitureArray[j].ID && FurnitureArray[j].Data.ParentID != FurnitureArray[i].ID)
                        {
                            int clearanceCollision = ClearenceAreaCollision(FurnitureArray[i], FurnitureArray[j]);
                            Penalty += clearanceCollision;
                        }
                    }
                }
            }
        }


        private int OutOfBoundsDeterminer(GeneralFurniture furniture)
        {
            int fine = 0;

            for (int j = 0; j < furniture.Vertices.GetLength(0); j++)
            {
                if (Math.Round(furniture.Vertices[j, 0], 5) > ContainerWidth || Math.Round(furniture.Vertices[j, 1], 5) > ContainerHeight ||
                    Math.Round(furniture.Vertices[j, 0], 5) > ContainerHeight || Math.Round(furniture.Vertices[j, 1], 5) > ContainerWidth ||
                    Math.Round(furniture.Vertices[j, 0], 5) < 0 || Math.Round(furniture.Vertices[j, 1], 5) < 0)
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

            decimal[,] arrayOfVerticesOne = new decimal[item1.Vertices.GetLength(0), 2];
            decimal[,] arrayOfVerticesTwo = new decimal[item2.Vertices.GetLength(0), 2];

            for (int j = 0; j < item1.Vertices.GetLength(0); j++)
            {
                for (int k = 0; k < item1.Vertices.GetLength(1); k++)
                {
                    arrayOfVerticesOne[j, k] = Math.Round(item1.Vertices[j, k], 5);
                    arrayOfVerticesTwo[j, k] = Math.Round(item2.Vertices[j, k], 5);
                }
            }

            for (int i = 0; i < item2.Vertices.GetLength(0); i++)
            {
                decimal[] pointOne = new decimal[2] { Math.Round(item1.Vertices[i, 0], 5), Math.Round(item1.Vertices[i, 1], 5) };
                decimal[] pointTwo = new decimal[2] { Math.Round(item2.Vertices[i, 0], 5), Math.Round(item2.Vertices[i, 1], 5) };

                if (DetermineCollision(arrayOfVerticesOne, pointTwo) || DetermineCollision(arrayOfVerticesTwo, pointOne))
                {
                    item2.IsCollided = true;
                    item1.IsCollided = true;
                    collided = true;
                    penalty += 100;
                }
            }

            decimal[] centerOne = new decimal[] { item1.Center[0], item1.Center[1] };
            decimal[] centerTwo = new decimal[] { item2.Center[0], item2.Center[1] };
            if (DetermineCollision(arrayOfVerticesOne, centerTwo) || DetermineCollision(arrayOfVerticesTwo, centerOne))
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

        private int ClearenceAreaCollision(GeneralFurniture item1, GeneralFurniture item2)
        {
            int penalty;

            penalty = ClearenceAreaCollisionProccess(item1, item2);
            if (penalty != 0)
            {
                return penalty;
            }

            penalty = ClearenceAreaCollisionProccess(item2, item1);
            if (penalty != 0)
            {
                return penalty;
            }

            return penalty;
        }

        private int ClearenceAreaCollisionProccess(GeneralFurniture item1, GeneralFurniture item2)
        {
            int penalty = 0;

            if (DetermineCollision == null)
                return penalty;

            decimal[,] arrayOfVertices = new decimal[item1.ClearanceArea.GetLength(0), 2];

            for (int j = 0; j < item1.ClearanceArea.GetLength(0); j++)
            {
                for (int k = 0; k < item1.ClearanceArea.GetLength(1); k++)
                {
                    arrayOfVertices[j, k] = Math.Round(item1.ClearanceArea[j, k], 5);
                }
            }

            for (int i = 0; i < item2.Vertices.GetLength(0); i++)
            {
                decimal[] point = new decimal[] { Math.Round(item2.Vertices[i, 0], 5), Math.Round(item2.Vertices[i, 1], 5) };
                if (DetermineCollision(arrayOfVertices, point))
                {
                    penalty += 10;
                }
            }

            return penalty;
        }

        private double NearWallPenalty(GeneralFurniture item)
        {
            if (item.Flags.NearWall < 0)
                return 0;

            Wall direction = DetermineClosestWall(item);
            double fine = 0;
            decimal distance = 0;
            decimal distancePercentage = 0;
            decimal distanceToSide;

            switch (direction)
            {
                case Wall.Left:
                    if (item.Rotation == 0 || item.Rotation == 180)
                    {
                        distanceToSide = item.Depth / 2;
                    }
                    else
                    {
                        distanceToSide = item.FrontWidth / 2;
                    }

                    if (item.Center[0] - distanceToSide > item.Flags.NearWall)
                    {
                        distance = item.Center[0] - distanceToSide - item.Flags.NearWall;
                        distancePercentage = distance / (ContainerWidth / 2);
                    }
                    if (item.Rotation != 0)
                    {
                        Penalty += 5;
                    }
                    break;

                case Wall.Right:
                    if (item.Rotation == 0 || item.Rotation == 180)
                    {
                        distanceToSide = item.Depth / 2;
                    }
                    else
                    {
                        distanceToSide = item.FrontWidth / 2;
                    }

                    if (item.Center[0] + distanceToSide < ContainerWidth - item.Flags.NearWall)
                    {
                        distance = ContainerWidth + distanceToSide - item.Flags.NearWall - item.Center[0];
                        distancePercentage = distance / (ContainerWidth / 2);
                    }
                    if (item.Rotation != 180)
                    {
                        Penalty += 5;
                    }
                    break;

                case Wall.Up:
                    if (item.Rotation == 0 || item.Rotation == 180)
                    {
                        distanceToSide = item.FrontWidth / 2;
                    }
                    else
                    {
                        distanceToSide = item.Depth / 2;
                    }

                    if (item.Center[1] - distanceToSide > item.Flags.NearWall)
                    {
                        distance = item.Center[1] - item.Flags.NearWall;
                        distancePercentage = distance / (ContainerHeight / 2);
                    }
                    if (item.Rotation != 90)
                    {
                        Penalty += 5;
                    }
                    break;

                case Wall.Down:
                    if (item.Rotation == 0 || item.Rotation == 180)
                    {
                        distanceToSide = item.FrontWidth / 2;
                    }
                    else
                    {
                        distanceToSide = item.Depth / 2;
                    }

                    if (item.Center[0] + distanceToSide < ContainerHeight - item.Flags.NearWall)
                    {
                        distance = ContainerHeight - item.Flags.NearWall - item.Center[1];
                        distancePercentage = distance / (ContainerHeight / 2);
                    }
                    if (item.Rotation != 270)
                    {
                        Penalty += 5;
                    }
                    break;
            }

            distancePercentage = Math.Round(distancePercentage, 5);

            if (distance != 0)
            {
                fine = (double)distancePercentage * 10;
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

                item.ClearanceArea[i, 0] += centerDeltaX;
                item.ClearanceArea[i, 1] += centerDeltaY;
            }
        }

        public int SafeMove(GeneralFurniture item, decimal x, decimal y)
        {
            Move(item, x, y);

            OutOfBoundsDeterminer(item);
            if (item.IsOutOfBounds == true)
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
                item.Vertices[i, 0] = Math.Round(item.Vertices[i, 0], 5);
                item.Vertices[i, 1] = Math.Round(item.Vertices[i, 1], 5);

                item.ClearanceArea[i, 0] = Math.Round(item.ClearanceArea[i, 0], 5);
                item.ClearanceArea[i, 1] = Math.Round(item.ClearanceArea[i, 1], 5);
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
                        Collision(item, FurnitureArray[i]);
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