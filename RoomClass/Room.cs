using Furniture;
using Interfaces;

namespace Rooms
{
    //TODO: Integration with Zones class once (semi?)finished.
    //TODO: Should this class handle rotation and movement of IPolygon objects (i.e. GeneralFurniture)? Discuss.
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

        public GeneralFurniture[] FurnitureArray { get; private set; }
        private List<GeneralFurniture> Doors { get; set; }
        private List<GeneralFurniture>? Windows { get; set; }
        public int[,] RoomArray { get; set; }
        public int ContainerHeight { get; private set; }
        public int ContainerWidth { get; private set; }
        public double Penalty { get; set; }
        //List<Zones> ZonesList { get; private set; } //DEW EET
        public bool WindowsInRoom { get; private set; }
        #endregion


        #region Delegates
        //public delegate void PathFinder(int[,] space, int entryCoordX, int entryCoordY, int desitnationX, int destinationY);
        public delegate bool CollisionDeterminer(decimal[,] vertices, decimal[] point);
        public CollisionDeterminer? DetermineCollision { get; set; }

        public delegate void VertexRotation(ref decimal x, ref decimal y, double radians, int centerX, int centerY);
        public VertexRotation? RotateVertex { get; set; }
        #endregion


        #region Constructor
        public Room(int height, int width, List<GeneralFurniture> doors, List<GeneralFurniture> items, bool windowed, List<GeneralFurniture>? windows = null)
        {
            ContainerHeight = height;
            ContainerWidth = width;
            RoomArray = new int[width, height];
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

        public void Mutate()
        {
            /*int randomNum = new Random().Next(1000);
            Console.WriteLine("Current id mutatuing " + randomNum);*/
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
                break;
            }
            //Console.WriteLine("finished id mutatuing " + randomNum);
        }

        public IPolygonGenesContainer Crossover(IPolygonGenesContainer parent)
        {
            if (parent is not Room roomParent)
            {
                return parent;
            }
            Room child = new(this.ContainerHeight, this.ContainerWidth, new List<GeneralFurniture>(), new List<GeneralFurniture>(), this.WindowsInRoom)
            {
                FurnitureArray = new GeneralFurniture[this.FurnitureArray.Length]
            };
            List<int> indexesToIgnore = new();

            for (int i = 0; i < this.FurnitureArray.Length; i++)
            {
                if (indexesToIgnore.Contains(i))
                    continue;
                if (i % 2 == 0)
                {
                    child.FurnitureArray[i] = (GeneralFurniture)this.FurnitureArray[i].Clone();
                    if (FurnitureArray[i].Data.ParentIndex != null)
                    {
                        indexesToIgnore.Add((int)FurnitureArray[i].Data.ParentIndex);
                        child.FurnitureArray[(int)FurnitureArray[i].Data.ParentIndex] = FurnitureArray[(int)FurnitureArray[i].Data.ParentIndex];
                    }
                    if (FurnitureArray[i].Data.ChildIndex != null)
                    {
                        indexesToIgnore.Add((int)FurnitureArray[i].Data.ChildIndex);
                        child.FurnitureArray[(int)FurnitureArray[i].Data.ChildIndex] = FurnitureArray[(int)FurnitureArray[i].Data.ChildIndex];
                    }
                }
                else
                {
                    child.FurnitureArray[i] = (GeneralFurniture)roomParent.FurnitureArray[i].Clone();
                    if (roomParent.FurnitureArray[i].Data.ParentIndex != null)
                    {
                        indexesToIgnore.Add((int)roomParent.FurnitureArray[i].Data.ParentIndex);
                        child.FurnitureArray[(int)roomParent.FurnitureArray[i].Data.ParentIndex] = roomParent.FurnitureArray[(int)roomParent.FurnitureArray[i].Data.ParentIndex];
                    }
                    if (roomParent.FurnitureArray[i].Data.ChildIndex != null)
                    {
                        indexesToIgnore.Add((int)roomParent.FurnitureArray[i].Data.ChildIndex);
                        child.FurnitureArray[(int)roomParent.FurnitureArray[i].Data.ChildIndex] = roomParent.FurnitureArray[(int)roomParent.FurnitureArray[i].Data.ChildIndex];
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

        public void Scatter(GeneralFurniture item)
        {
            decimal x = new Random().Next(-1, 1);
            if (new Random().Next(10) > 8)
                x += 3;

            decimal y = new Random().Next(-1, 1);
            if (new Random().Next(10) > 8)
                y += 3;

            if (item.Data.ParentIndex != null)
            {
                Move(FurnitureArray[(int)item.Data.ParentIndex], x, y);
                if (OutOfBoundsDeterminer(FurnitureArray[(int)item.Data.ParentIndex]) != 0)
                {
                    Move(FurnitureArray[(int)item.Data.ParentIndex], -x, -y);
                    return;
                }
            }
            if (item.Data.ChildIndex != null)
            {
                Move(FurnitureArray[(int)item.Data.ChildIndex], x, y);
                if (OutOfBoundsDeterminer(FurnitureArray[(int)item.Data.ChildIndex]) != 0)
                {
                    Move(FurnitureArray[(int)item.Data.ChildIndex], -x, -y);
                    return;
                }
            }
            Move(item, x, y);
            if (OutOfBoundsDeterminer(item) != 0)
            {
                Move(item, -x, -y);
                return;
            }
            for (int i = 0; i < FurnitureArray.Length; i++)
            {
                if (item != FurnitureArray[i] && (Collision(item, FurnitureArray[i]) != 0 || Collision(FurnitureArray[i], item) != 0))
                {
                    Move(item, -x, -y);
                    return;
                }
            }
            Console.WriteLine("successful " + item.Name + " move");
            if(item.IsCollided)
                Console.WriteLine(item.IsCollided);
        }

        public void RandomRotation(GeneralFurniture item)
        {
            int rotateFor = new Random().Next(10);
            if (item.Data.ParentIndex != null)
            {
                Rotate(FurnitureArray[(int)item.Data.ParentIndex], rotateFor);
                if (OutOfBoundsDeterminer(FurnitureArray[(int)item.Data.ParentIndex]) != 0)
                {
                    Rotate(FurnitureArray[(int)item.Data.ParentIndex], rotateFor);
                    return;
                }
            }
            if (item.Data.ChildIndex != null)
            {
                Rotate(FurnitureArray[(int)item.Data.ChildIndex], rotateFor);
                if (OutOfBoundsDeterminer(FurnitureArray[(int)item.Data.ChildIndex]) != 0)
                {
                    Rotate(FurnitureArray[(int)item.Data.ChildIndex], rotateFor);
                    return;
                }
            }
            Rotate(item, rotateFor);
            if (OutOfBoundsDeterminer(item) != 0)
            {
                Rotate(item, -rotateFor);
                return;
            }
            for (int i = 0; i < FurnitureArray.Length; i++)
            {
                if (item != FurnitureArray[i] && (Collision(item, FurnitureArray[i]) != 0 || Collision(FurnitureArray[i], item) != 0))
                {
                    Rotate(item, -rotateFor);
                    return;
                }
            }
        }

        public void WallAlignment(GeneralFurniture item)
        {
            bool left = false;
            bool right = false;
            bool up = false;
            bool down = false;


            decimal widthValue = item.Center[0] / ContainerWidth;
            decimal heightValue = item.Center[1] / ContainerHeight;


            if (widthValue > (decimal)0.5)
                right = true;
            else
                left = true;

            if (heightValue > (decimal)0.5)
                down = true;
            else
                up = true;


            if (widthValue < (decimal)0.5)
            {
                widthValue = 1 - widthValue;
            }
            if (widthValue < (decimal)0.5)
            {
                heightValue = 1 - heightValue;
            }


            if (widthValue < heightValue)
            {
                left = false; right = false;
            }
            else
            {
                up = false; down = false;
            }


            if (left)
            {
                Move(item, -item.Center[0] + item.Width / 2, 0);
                for (int i = 0; i < FurnitureArray.Length; i++)
                {
                    if (item != FurnitureArray[i] && (Collision(item, FurnitureArray[i]) != 0 || Collision(FurnitureArray[i], item) != 0))
                    {
                        Move(item, -(-item.Center[0] + item.Width / 2), 0);
                        return;
                    }
                    if (OutOfBoundsDeterminer(item) != 0)
                    {
                        Move(item, -(-item.Center[0] + item.Width / 2), 0);
                        return;
                    }
                }
            }
            if (right)
            {
                Move(item, ContainerWidth - item.Center[0] - item.Width / 2, 0);
                for (int i = 0; i < FurnitureArray.Length; i++)
                {
                    if (item != FurnitureArray[i] && (Collision(item, FurnitureArray[i]) != 0 || Collision(FurnitureArray[i], item) != 0))
                    {
                        Move(item, -(ContainerWidth - item.Center[0] - item.Width / 2), 0);
                        return;
                    }
                    if (OutOfBoundsDeterminer(item) != 0)
                    {
                        Move(item, -(ContainerWidth - item.Center[0] - item.Width / 2), 0);
                        return;
                    }
                }
            }
            if (up)
            {
                Move(item, -item.Center[1] + item.Height / 2, 0);
                for (int i = 0; i < FurnitureArray.Length; i++)
                {
                    if (item != FurnitureArray[i] && (Collision(item, FurnitureArray[i]) != 0 || Collision(FurnitureArray[i], item) != 0))
                    {
                        Move(item, -(-item.Center[1] + item.Height / 2), 0);
                        return;
                    }
                    if (OutOfBoundsDeterminer(item) != 0)
                    {
                        Move(item, -(-item.Center[1] + item.Height / 2), 0);
                        return;
                    }
                }
            }
            if (down)
            {
                Move(item, ContainerHeight - item.Center[1] - item.Height / 2, 0);
                for (int i = 0; i < FurnitureArray.Length; i++)
                {
                    if (item != FurnitureArray[i] && (Collision(item, FurnitureArray[i]) != 0 || Collision(FurnitureArray[i], item) != 0))
                    {
                        Move(item, -(ContainerHeight - item.Center[1] - item.Height / 2), 0);
                        return;
                    }
                    if (OutOfBoundsDeterminer(item) != 0)
                    {
                        Move(item, -(ContainerHeight - item.Center[1] - item.Height / 2), 0);
                        return;
                    }
                }
            }

            Console.WriteLine("cunt");

            item.Data.ParentIndex = null;
            item.Data.ChildIndex = null;
        }

        public void MoveObjectToObject(GeneralFurniture item)
        {
            int index = 0;
            //double minDistance = -1;
            List<int> candidates = new();
            for (int i = 0; i < FurnitureArray.Length; i++)
            {
                if (FurnitureArray[i].ID != item.ID)
                {
                    candidates.Add(i);

                    /*double distance = Math.Sqrt(Math.Pow((double)(item.Center[0] - FurnitureArray[i].Center[0]), 2) +
                       Math.Pow((double)(item.Center[1] - FurnitureArray[i].Center[1]), 2));

                    if (minDistance == -1 || distance < minDistance)
                    {
                        minDistance = distance;
                        index = i;
                    }*/
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
            Move(item, (pointToMoveToX - oldCenterX), (pointToMoveToY - oldCenterY));
            if (OutOfBoundsDeterminer(item) != 0)
            {
                Move(item, -(pointToMoveToX - oldCenterY), -(pointToMoveToY - oldCenterY));
                return;
            }
            for (int i = 0; i < FurnitureArray.Length; i++)
            {
                if (item != FurnitureArray[i] && (Collision(item, FurnitureArray[i]) != 0 || Collision(FurnitureArray[i], item) != 0))
                {
                    Move(item, -(pointToMoveToX - oldCenterY), -(pointToMoveToY - oldCenterY));
                    return;
                }
            }
            item.Data.ParentIndex = null;
            item.Data.ChildIndex = null;
        }

        public void MoveToParent(GeneralFurniture item)
        {
            if (item.Data.ParentID == -1)
                return;

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
                return;

            decimal pointToMoveToX = Math.Round(FurnitureArray[index].Center[0] + (decimal)Math.Cos(FurnitureArray[index].Rotation) * FurnitureArray[index].Width);
            decimal pointToMoveToY = Math.Round(FurnitureArray[index].Center[1] + (decimal)Math.Sin(FurnitureArray[index].Rotation) * FurnitureArray[index].Width);

            decimal oldCenterX = item.Center[0];
            decimal oldCenterY = item.Center[1];
            Move(item, (pointToMoveToX - oldCenterX), (pointToMoveToY - oldCenterY));
            if (OutOfBoundsDeterminer(item) != 0)
            {
                Move(item, (item.Center[0] - pointToMoveToX), (item.Center[1] - pointToMoveToY));
                return;
            }
            Rotate(item, FurnitureArray[index].Rotation - item.Rotation);

            item.Data.ParentIndex = index;
            FurnitureArray[index].Data.ChildIndex = FurnitureArray.ToList().IndexOf(item);

        }

        public void Alighnment(GeneralFurniture item)
        {
            GeneralFurniture alighnTo;
            while (true)
            {
                alighnTo = FurnitureArray[new Random().Next(FurnitureArray.Length)];

                if (alighnTo != item)
                    break;
            }

            Rotate(item, alighnTo.Rotation - item.Rotation);
        }

        //TODO Improve penalty evaluation by implementing a better method for more flexibility
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
                if (furniture.Vertices[j, 0] > (decimal)ContainerWidth || furniture.Vertices[j, 1] > (decimal)ContainerHeight || furniture.Vertices[j, 0] < 0 || furniture.Vertices[j, 1] < 0)
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
            int penalty = 0;
            bool collided = false;
            if (DetermineCollision == null)
                return penalty;
            for (int i = 0; i < item2.Vertices.GetLength(0); i++)
            {
                decimal[] point = new decimal[] { item2.Vertices[i, 0], item2.Vertices[i, 1] };

                if (DetermineCollision(item1.Vertices, point))
                {
                    item2.IsCollided = true;
                    item1.IsCollided = true;
                    collided = true;
                    penalty += 10;
                }
            }
            decimal[] center = new decimal[] { item2.Center[0], item2.Center[1] };

            if (DetermineCollision(item1.Vertices, center))
            {
                item2.IsCollided = true;
                item1.IsCollided = true;
                collided = true;
                penalty += 10;
            }

            if (!collided)
            {
                item1.IsCollided = false;
                item2.IsCollided = false;
            }
            return penalty;
        }

        private int NearWallPenalty(GeneralFurniture furniture)
        {
            if (furniture.Flags.NearWall < 0)
                return 0;

            int fine = 0;
            bool check = false;

            for (int i = 0; i < furniture.Vertices.GetLength(0); i++)
            {
                if (furniture.Vertices[i, 1] <= (0 + furniture.Flags.NearWall))
                {
                    check = true;
                    if (!(furniture.Rotation >= 260 && furniture.Rotation <= 280))
                        fine += 5;
                    break;
                }
                if (furniture.Vertices[i, 0] <= (0 + furniture.Flags.NearWall))
                {
                    check = true;
                    if (!((furniture.Rotation >= 350 && furniture.Rotation < 360) ||
                        (furniture.Rotation >= 0 && furniture.Rotation <= 10)))
                        fine += 5;
                    break;
                }
                if (furniture.Vertices[i, 1] >= (ContainerHeight - furniture.Flags.NearWall))
                {
                    check = true;
                    if (!(furniture.Rotation >= 80 && furniture.Rotation <= 100))
                        fine += 5;
                    break;
                }
                if (furniture.Vertices[i, 0] >= (ContainerWidth - furniture.Flags.NearWall))
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

        #endregion
    }
}