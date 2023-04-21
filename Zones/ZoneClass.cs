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
        public int Width { get; set; }
        public int Height { get; set; }
        public decimal[] Center { get; private set; }
        public decimal[,] Vertices { get; private set; }
        public bool isStorage { get; private set; }


        public Zone(List<GeneralFurniture> furnitures, string zoneName)
        {
            isStorage = false;
            Name = zoneName;
            Furnitures = furnitures.Where(p => p.ZoneName == zoneName).ToList();

            Width = Furnitures.Select(p => p.Width).Sum();
            Height = Furnitures.Select(p => p.Height).Sum();
            Area = Math.Sqrt(Width * Height);
            FurnitureArea = Furnitures.Select(p => p.Width * p.Height).Sum();

            Center = new decimal[2];
            Center[0] = (decimal)Width / 2;
            Center[1] = (decimal)Height / 2;

            Vertices = new decimal[4, 2];

            VertexManipulator.VertexResetting(Vertices, Center, Width, Height);

            if (DetermineZoneType())
                isStorage = true;

        }

        public Zone(Zone prevZone)
        {
            Center = new decimal[2];
            Vertices = new decimal[4, 2];


            Name = prevZone.Name;
            Furnitures = prevZone.Furnitures;
            Width = prevZone.Width;
            Height = prevZone.Height;
            Area = prevZone.Area;
            FurnitureArea = prevZone.FurnitureArea;
            Center = prevZone.Center;
            Vertices = prevZone.Vertices;
            //Array.Copy(prevZone.Center, Center, prevZone.Center.Length);
            //Array.Copy(prevZone.Vertices, Vertices, prevZone.Vertices.Length);
            isStorage = prevZone.isStorage;
        }

        //TODO InitializeZones() can be used just once


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
            Width = (int)Math.Floor(deltaW);
            Height = (int)Math.Floor(deltaH);
            Area = Width * Height;
        }

        private bool DetermineZoneType() => Name.ToLower() == "storage" ? true : false;
    }
}

