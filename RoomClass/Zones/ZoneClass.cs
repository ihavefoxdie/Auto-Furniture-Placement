namespace RoomClass.Zones
{
    public class Zone
    {
        public string Name { get; set; }
        public int Length { get; set; }
        public int Height { get; set; }
        public int Area { get; set; }

        //TODO Максимальная длина и ширина среди набора мебели (для сжатия во время пожара)

        public Zone(List<Furniture> furnitures, string zoneName)
        {
            Name = zoneName;
            Length = furnitures.Where(p => p.Zone == zoneName).Select(p => p.Length).Sum();
            Height = furnitures.Where(p => p.Zone == zoneName).Select(p => p.Height).Sum();
            Area = Length * Height;
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

    }

 
}

