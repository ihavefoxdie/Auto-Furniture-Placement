namespace Interfaces
{
    public interface IPolygon
    {
        public int ID { get; }
        public int Width { get; }
        public int Height { get; }
        public decimal[] Center { get; }
        public decimal[,] Vertices { get; }
        public string Name { get; }
    }
}
