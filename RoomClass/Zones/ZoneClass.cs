namespace RoomClass.Zones
{
    public class Zone : IPolygon
    {
        public int ID { get; }
        public string Name { get; set; }
        public double Area { get; set; }
        public List<GeneralFurniture> Furnitures { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public decimal[] Center { get; private set; }
        public decimal[,] Vertices { get; private set; }

        public Zone(List<GeneralFurniture> furnitures, string zoneName)
        {
            Name = zoneName;
            Furnitures = furnitures.Where(p => p.Zone == zoneName).ToList();
            Width = Furnitures.Select(p => p.Width + 2).Sum();
            Height = Furnitures.Select(p => p.Height + 2).Sum();
            Area = Math.Floor(Math.Sqrt(Width * Height));
            Center = new decimal[2];
            Center[0] = (decimal)Width / 2;     
            Center[1] = (decimal)Height / 2;     
            Vertices = new decimal[4, 2];

            Vertices[0, 1] = Height;                             // ← ↑

            Vertices[1, 0] = Width; Vertices[1, 1] = Height;    // → ↑

            Vertices[2, 0] = Width;                             // → ↓
        }
        
        //TODO Method can be used just once
        public static List<Zone> InitializeZones(List<GeneralFurniture> furnitures)
        {
            List<Zone> list = new List<Zone>();

            // List contains distinct zones
            List<string> unique = new List<string>();

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

        public void Rotate(int angle)
        {
            throw new NotImplementedException();
        }

        //TODO Zone resizing method
    }
}

