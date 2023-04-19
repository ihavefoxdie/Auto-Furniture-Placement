using Furniture;
using Interfaces;

namespace Zones
{

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
            Furnitures = furnitures.Where(p => p.Zone == zoneName).ToList();
            Width = Furnitures.Select(p => p.Width).Sum();
            Height = Furnitures.Select(p => p.Height).Sum();
            Area = Math.Sqrt(Width * Height);
            FurnitureArea = Furnitures.Select(p => p.Width * p.Height).Sum();

            Center = new decimal[2];
            Center[0] = (decimal)Width / 2;
            Center[1] = (decimal)Height / 2;

            Vertices = new decimal[4, 2];
            Vertices[0, 1] = Height;                             // ← ↑
            Vertices[1, 0] = Width; Vertices[1, 1] = Height;    // → ↑
            Vertices[2, 0] = Width;                             // → ↓

            if (DetermineZoneType())
                isStorage = true;


        }

        public Zone(Zone prevZone)
        {
            Center = new decimal[2];
            Vertices = new decimal[4, 2];

            Name = prevZone.Name;
            Furnitures = prevZone.Furnitures;
            Width = prevZone.Width + 2;
            Height = prevZone.Height + 2;
            Area = Width * Height;
            FurnitureArea = prevZone.FurnitureArea;
            Array.Copy(prevZone.Center, Center, prevZone.Center.Length);
            Array.Copy(prevZone.Vertices, Vertices, prevZone.Vertices.Length);
            isStorage = prevZone.isStorage;

        }

        //TODO Method can be used just once
        public static List<Zone> InitializeZones(List<GeneralFurniture> furnitures)
        {
            List<Zone> list = new();

            // List contains distinct zones
            List<string> unique = new();

            foreach (var item in furnitures)
            {
                unique.Add(item.Zone);
            }

            unique = unique.Distinct().ToList();

            foreach (var item in unique)
            {
                Zone zone = new(furnitures, item);
                list.Add(zone);
            }

            return list;
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
            Width = (int)Math.Floor(deltaW);
            Height = (int)Math.Floor(deltaH);
        }

        private bool DetermineZoneType()
        {
            if (Name == "storage")
                return true;
            return false;

        }

    }
}

