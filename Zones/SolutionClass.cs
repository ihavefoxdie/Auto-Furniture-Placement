using Interfaces;
using Vertex;
using Zones;


namespace RoomClass.Zones
{

    internal class SolutionClass
    {
        public List<AnnealingZone> Zones { get; set; }
        public int Aisle { get; set; }
        public double Cost { get; set; }

        public int RoomWidth { get; set; }
        public int RoomHeight { get; set; }

        public SolutionClass(List<AnnealingZone> zones, int aisle, int roomWidth, int roomHeight)
        {
            Aisle = aisle;
            RoomWidth = roomWidth;
            RoomHeight = roomHeight;


            Cost = FindCost();


        }


        public SolutionClass GenerateNeighbour(double maxStep)
        {

            throw new NotImplementedException();

        }

        public double FindCost()
        {
            double cost = 0;

            cost += OverlappingPenalty();
            cost += FreeSpacePenalty();
            cost += ZoneShapePenalty();



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

    }
}
