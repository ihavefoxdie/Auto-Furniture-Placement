using Interfaces;
using Vertex;
using Zones;

namespace RoomClass.Zones
{

    internal class SolutionClass
    {
        public List<AnnealingZone> Zones { get; set; }
        public double Cost { get; set; }


        public SolutionClass(List<AnnealingZone> zones, int aisle)
        {
            Cost = 0;
            Cost = FindCost(aisle);
        }


        public SolutionClass GenerateNeighbour(double maxStep)
        {

            throw new NotImplementedException();

        }

        public double FindCost(int aisle)
        {
            Cost += OverlappingPenalty();


            throw new NotImplementedException();
        }

        private double OverlappingPenalty()
        {
            double area = 0;

            for (int i = 0; i < Zones.Count - 1; i++)
            {
                for (int j = i + 1; j < Zones.Count; j++)
                {
                    if (VertexManipulator.DetermineCollision(Zones[i].Vertices, Zones[j].Center))
                    {
                        area += FindOverlapArea(Zones[i], Zones[j]);
                    }
                }
            }

            return Math.Sqrt(area);
        }

        private double FindOverlapArea<T>(T zone1, T zone2) where T : IPolygon
        {
            /*
    x1, y1 - левая нижняя точка первого прямоугольника   - D
    x2, y2 - правая верхняя точка первого прямоугольника  - B
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
    }
}
