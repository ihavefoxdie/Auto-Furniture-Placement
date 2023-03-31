using Interfaces;
using Zones;

namespace RoomClass.Zones
{

    //TODO Mandatory item is adding FindCost() in ctor
    internal class SolutionClass
    {

        public List<Zone> Zones { get; set; }

        public SolutionClass()
        {
            Cost = 0;
        }

        public double Cost { get; set; }

        public SolutionClass GenerateNeighbour(double maxStep)
        {

            throw new NotImplementedException();

        }

        public double FindCost()
        {


            throw new NotImplementedException();
        }

        private double OverlappingPenalty(List<Zone> zones)
        {
            for (int i = 0; i < zones.Count; i++)
            {
                for (int j = 0; j < zones.Count; j++)
                {

                    if (i != j)
                    {
                        //if (Room.Collision(zones[i], zones[j]))
                        //{


                        //}
                    }



                }
            }

            throw new NotImplementedException();

        }

        public static decimal FindOverlapArea<T> (T zone1, T zone2) where T : IPolygon
        {
            /*
    x1, y1 - левая нижняя точка первого прямоугольника   - D
    x2, y2 - правая верхняя точка первого прямоугольника  - B
    x3, y3 - левая нижняя точка второго прямоугольника
    x4, y4 - правая верхняя точка второго прямоугольника
*/


            decimal left = Math.Max(zone1.Vertices[3, 0], zone2.Vertices[3, 0]);
            decimal top = Math.Min(zone1.Vertices[1,1], zone2.Vertices[1, 1]);
            decimal right = Math.Min(zone1.Vertices[1, 0], zone2.Vertices[1, 0]);
            decimal bottom = Math.Max(zone1.Vertices[3, 1], zone2.Vertices[3, 1]);

            decimal width = right - left;
            decimal height = top - bottom;

            if (width < 0 || height < 0)
                return 0;

            return width * height;
        }
    }
}
