using Furniture;
using Interfaces;
using Vertex;
using Zones;


namespace RoomClass.Zones
{

    public class SolutionClass
    {
        public List<AnnealingZone> Zones { get; set; }
        private List<GeneralFurniture> Doors { get; set; }

        public int Aisle { get; set; }
        public double Cost { get; set; }

        public int RoomWidth { get; private set; }
        public int RoomHeight { get; private set; }


        public SolutionClass(List<AnnealingZone> zones, int aisle, int roomWidth, int roomHeight, List<GeneralFurniture> doors)
        {
            Aisle = aisle;
            RoomWidth = roomWidth;
            RoomHeight = roomHeight;
            Doors = doors;
            Zones = zones;
            Cost = FindCost();
        }


        public SolutionClass GenerateNeighbour(double maxStep)
        {
            Random random = new Random();
            int randomZoneNumber = random.Next(Zones.Count);
            //Take a random zone
            //TODO To make sure about deep copy here
            AnnealingZone neighbourZone;
            var deepZonesCopy = Zones.ToList();

            for (int i = random.Next(1, Zones.Count); i > 0; i--)
            {
                while (true)
                {
                    neighbourZone = new AnnealingZone(Zones[randomZoneNumber]);
                    RandomizeZone(neighbourZone, (decimal)maxStep);
                    VertexManipulator.VertexResetting(neighbourZone.Vertices, neighbourZone.Center, neighbourZone.Width, neighbourZone.Height);
                    if (IsZoneInsideRoom(neighbourZone))
                    {
                        deepZonesCopy[randomZoneNumber] = neighbourZone;
                        break;
                    }
                    randomZoneNumber = random.Next(Zones.Count);
                }
            }
            //Do a random action (moving or resizing)

            return new SolutionClass(deepZonesCopy, Aisle, RoomWidth, RoomHeight, Doors);

        }

        public double FindCost()
        {
            double cost = 0;

            cost += OverlappingPenalty();
            cost += FreeSpacePenalty();
            cost += ZoneShapePenalty();
            cost += SpaceRatioPenalty();
            cost += ByWallPenalty();
            cost += DoorSpacePenalty();

            return cost;
        }

        private double OverlappingPenalty()
        {
            double area = 0;

            for (int i = 0; i < Zones.Count - 1; i++)
            {
                for (int j = i + 1; j < Zones.Count; j++)
                {
                    VertexManipulator.VertexExpanding(Zones[j].Vertices, Aisle);

                    if (DeterminRectangleCollision(Zones[i], Zones[j]))
                    {
                        area += FindOverlapArea(Zones[i], Zones[j]);
                    }

                    VertexManipulator.VertexExpanding(Zones[j].Vertices, -Aisle);
                }
            }

            return Math.Sqrt(area);
        }

        private double FreeSpacePenalty()
        {
            double area = 0;
            foreach (var item in Zones)
            {
                item.Resize(Aisle, Aisle);
                area += item.Area;
                item.Resize(-Aisle, -Aisle);
            }
            return Math.Sqrt(RoomHeight * RoomWidth - area);
        }

        private double ZoneShapePenalty()
        {
            double penalty = 0;

            foreach (var item in Zones)
            {
                if (item.ExtendedHeight >= item.ExtendedWidth * 3 || item.ExtendedWidth >= item.ExtendedHeight * 3)
                {
                    decimal maxDim = Math.Max(item.ExtendedWidth, item.ExtendedHeight);
                    decimal minDim = Math.Min(item.ExtendedWidth, item.ExtendedHeight);

                    penalty += (double)((maxDim - 3 * minDim) / 4);
                }
            }
            return penalty;
        }

        private double SpaceRatioPenalty()
        {
            double penalty = 0;
            double allFurnituresArea = Zones.Select(x => x.FurnitureArea).Sum();
            double allZonesArea = Zones.Select(x => x.Area).Sum();

            foreach (var item in Zones)
            {
                penalty += Math.Abs((item.FurnitureArea / allFurnituresArea) - (item.Area / allZonesArea));
            }
            return penalty;
        }

        private double ByWallPenalty()
        {
            double penalty = 0;
            List<double> wall = new List<double>();

            foreach (var item in Zones)
            {
                wall.Clear();

                for (int i = 0; i < 4; i++)
                {
                    if (IsVertexInsideRoom(item.Vertices[i, 0], item.Vertices[i, 1]))
                    {
                        wall = FindDistances(item.Vertices);
                    }

                    else throw new Exception($"Zone goes beyond the Room[W:{RoomWidth}  H:{RoomHeight}]\n \tZoneId: {item.Name}\tVerticeNumber : {i}");
                }

                penalty += wall[0] + wall[1] + wall[2] / 2;
            }

            return penalty;
        }

        private double DoorSpacePenalty()
        {

            double overlapArea = 0;

            foreach (var item in Doors)
            {
                VertexManipulator.VertexExpanding(item.Vertices, 0, Aisle);
            }


            foreach (var item in Zones)
            {
                foreach (var itemDoor in Doors)
                {
                    overlapArea += FindOverlapArea<IPolygon>(itemDoor, item);
                }

            }


            foreach (var item in Doors)
            {
                VertexManipulator.VertexExpanding(item.Vertices, 0, -Aisle);
            }

            return Math.Sqrt(overlapArea);
        }


        private bool IsVertexInsideRoom(decimal x, decimal y)
        {
            if (x > RoomWidth || x < 0)
                return false;

            if (y > RoomHeight || y < 0)
                return false;

            return true;
        }

        private bool IsZoneInsideRoom(IPolygon zone)
        {
            for (int i = 0; i < 4; i++)
            {
                if (!IsVertexInsideRoom(zone.Vertices[i, 0], zone.Vertices[i, 1]))
                    return false;
            }
            return true;
        }

        private List<double> FindDistances(decimal[,] vertices)
        {
            List<double> result = new List<double>();
            double distance = double.MaxValue;

            for (int i = 0; i < 4; i++)
            {
                distance = double.MaxValue;

                for (int j = 0; j < 2; j++)
                {
                    // X axis distance
                    if ((double)(RoomWidth - vertices[i, 0]) < distance)
                        distance = (double)(RoomWidth - vertices[i, 0]);

                    if ((double)(vertices[i, 0]) < distance)
                        distance = (double)(vertices[i, 0]);

                    // Y axis distance
                    if ((double)(RoomHeight - vertices[i, 1]) < distance)
                        distance = (double)(RoomWidth - vertices[i, 1]);

                    if ((double)(vertices[i, 1]) < distance)
                        distance = (double)(vertices[i, 1]);
                }
                result.Add(distance);
            }

            result.Remove(result.Max());
            return result;
        }


        private double FindOverlapArea<T>(T zone1, T zone2) where T : IPolygon
        {
            /*
    x1, y1 - левая нижняя точка первого прямоугольника
    x2, y2 - правая верхняя точка первого прямоугольника
    x3, y3 - левая нижняя точка второго прямоугольника
    x4, y4 - правая верхняя точка второго прямоугольника
            */


            decimal left = Math.Max(zone1.Vertices[3, 0], zone2.Vertices[3, 0]);
            decimal top = Math.Min(zone1.Vertices[1, 1], zone2.Vertices[1, 1]);
            decimal right = Math.Min(zone1.Vertices[1, 0], zone2.Vertices[1, 0]);
            decimal bottom = Math.Max(zone1.Vertices[3, 1], zone2.Vertices[3, 1]);

            decimal width = right - left;
            decimal height = top - bottom;

            if (width < 0 || height < 0)
                return 0;

            return (double)(width * height);
        }

        private bool DeterminRectangleCollision(AnnealingZone rect1, AnnealingZone rect2)
        {
            if (
                rect1.Center[0] - rect1.ExtendedWidth / 2 < rect2.Center[0] + rect2.ExtendedWidth / 2 &&
                rect1.Center[0] + rect1.ExtendedWidth / 2 > rect2.Center[0] - rect2.ExtendedWidth / 2 &&
                rect1.Center[1] - rect1.ExtendedHeight / 2 < rect2.Center[1] + rect2.ExtendedHeight / 2 &&
                rect1.ExtendedHeight / 2 + rect1.Center[1] > rect2.Center[1] - rect2.ExtendedHeight / 2
              )
                return true;
            return false;
        }

        private bool RandomBoolean()
        {
            Random random = new();
            if (random.Next(2) > 0)
            {
                return true;
            }
            return false;
        }

        private void RandomizeZone(AnnealingZone randomZone, decimal maxStep)
        {
            Random random = new();

            if (RandomBoolean())
            {
                //TODO Evade collision between zone and room for each of (resizing or moving)
                //resizing options

                if (randomZone.isStorage == true)
                {
                    RandomizeZone(randomZone, maxStep);
                    return;
                }

                switch (random.Next(6))
                {

                    case 0:
                        randomZone.Resize((decimal)maxStep, (decimal)maxStep);
                        break;

                    case 1:
                        randomZone.Resize(0, (decimal)maxStep);
                        break;

                    case 2:
                        randomZone.Resize((decimal)maxStep, 0);
                        break;

                    case 3:
                        randomZone.Resize(-(decimal)maxStep, -(decimal)maxStep);
                        break;

                    case 4:
                        randomZone.Resize(0, -(decimal)maxStep);
                        break;

                    case 5:
                        randomZone.Resize(-(decimal)maxStep, 0);
                        break;

                    default:
                        break;
                }

            }

            else
            {
                //moving options

                switch (random.Next(6))
                {

                    case 0:
                        randomZone.Center[0] += (decimal)maxStep;
                        break;

                    case 1:
                        randomZone.Center[1] += (decimal)maxStep;
                        break;

                    case 2:
                        randomZone.Center[0] += (decimal)maxStep;
                        randomZone.Center[1] += (decimal)maxStep;
                        break;

                    case 3:
                        randomZone.Center[0] -= (decimal)maxStep;
                        break;

                    case 4:
                        randomZone.Center[1] -= (decimal)maxStep;
                        break;

                    case 5:
                        randomZone.Center[0] -= (decimal)maxStep;
                        randomZone.Center[1] -= (decimal)maxStep;
                        break;

                    default:
                        break;
                }

            }


        }

    }
}
