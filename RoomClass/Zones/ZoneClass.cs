namespace RoomClass.Zones
{
    public class Zone : IPolygon
    {
        public string Name { get; set; }
        public double Area { get; set; }
        public List<Furniture> Furnitures { get; set; }
        public int Length { get; set; }
        public int Height { get; set; }
        public double[] Center { get; private set; }
        public double[,] Vertices { get; private set; }

        public Zone(List<Furniture> furnitures, string zoneName)
        {
            Name = zoneName;
            Furnitures = furnitures.Where(p => p.Zone == zoneName).ToList();
            Length = Furnitures.Select(p => p.Length + 2).Sum();
            Height = Furnitures.Select(p => p.Height + 2).Sum();
            Area = Math.Floor(Math.Sqrt(Length * Height));
            Center = new double[2];
            Center[0] = (double)Length / 2;     
            Center[1] = (double)Height / 2;     
            Vertices = new double[4, 2];

            Vertices[0, 1] = Height;                             // ← ↑

            Vertices[1, 0] = Length; Vertices[1, 1] = Height;    // → ↑

            Vertices[2, 0] = Length;                             // → ↓
        }
        
        //TODO Method can be used just once
        public static List<Zone> InitializeZones(List<Furniture> furnitures)
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

        public void Move(double centerDeltaX, double centerDeltaY)
        {
            Center[0] += centerDeltaX;
            Center[1] += centerDeltaY;

            for (int i = 0; i < Vertices.GetLength(0); i++)
            {
                Vertices[i, 0] += centerDeltaX;
                Vertices[i, 1] += centerDeltaY;
            }
        }

        //TODO Zone resizing method
    }


}

