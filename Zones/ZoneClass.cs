using Furniture;
using Interfaces;
using Vertex;

namespace Zones
{
    //TODO Determine how to create IDs for each zone
    public class Zone : IPolygon
    {
        public int ID { get; }
        public string Name { get; set; }
        public double Area { get; set; }
        public double FurnitureArea { get; set; }
        public List<GeneralFurniture> Furnitures { get; set; }
        public int Depth { get; set; }
        public int FrontWidth { get; set; }
        public decimal[] Center { get; set; }
        public decimal[,] Vertices { get; set; }
        public bool isStorage { get; private set; }


        public Zone(List<GeneralFurniture> furnitures, string zoneName)
        {
            isStorage = false;
            Name = zoneName;
            Furnitures = furnitures.Where(p => p.Data.Zone == zoneName).ToList();

            Depth = Furnitures.Select(p => p.Depth).Sum();
            FrontWidth = Furnitures.Select(p => p.FrontWidth).Sum();
            Area = Math.Sqrt(Depth * FrontWidth);
            FurnitureArea = Furnitures.Select(p => p.Depth * p.FrontWidth).Sum();

            Center = new decimal[2];
            Center[0] = (decimal)Depth / 2;
            Center[1] = (decimal)FrontWidth / 2;

            Vertices = new decimal[4, 2];

            VertexManipulator.VertexResetting(Vertices, Center, Depth, FrontWidth);

            if (DetermineZoneType())
                isStorage = true;

        }

        public Zone(Zone prevZone)
        {
            Center = new decimal[2];
            Vertices = new decimal[4, 2];


            Name = prevZone.Name;
            Furnitures = prevZone.Furnitures;
            Depth = prevZone.Depth;
            FrontWidth = prevZone.FrontWidth;
            Area = prevZone.Area;
            FurnitureArea = prevZone.FurnitureArea;
            Array.Copy(prevZone.Center, Center, prevZone.Center.Length);
            Array.Copy(prevZone.Vertices, Vertices, prevZone.Vertices.Length);  

            isStorage = prevZone.isStorage;
        }


        public void Move(decimal centerDeltaX, decimal centerDeltaY)
        {
            Center[0] += centerDeltaX;
            Center[1] += centerDeltaY;

            for (int i = 0; i < Vertices.GetLength(0); i++)
            {
                Vertices[i, 0] += centerDeltaX;
                Vertices[i, 1] += centerDeltaY;
            }
        }

        public virtual void Rotate(int angle)
        {
            throw new NotImplementedException();
        }

        public virtual void Resize(decimal deltaW, decimal deltaH)
        {
            Depth = (int)Math.Floor(deltaW);
            FrontWidth = (int)Math.Floor(deltaH);
            Area = Depth * FrontWidth;
        }

        private bool DetermineZoneType() => Name.ToLower() == "storage";
    }
}

