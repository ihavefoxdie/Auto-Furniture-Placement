namespace Interfaces
{
    public interface IPolygon
    {
        public int ID { get; }
        public int Depth { get; }
        public int FrontWidth { get; }
        public decimal[] Center { get; }
        public decimal[,] Vertices { get; }
        public string Name { get; }
    }
}
