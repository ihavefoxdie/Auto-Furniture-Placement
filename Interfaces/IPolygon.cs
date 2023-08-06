namespace Interfaces
{
    //TODO: We should probably make movement and rotation be handled by something else, not by the IPolygon objects themselves.
    public interface IPolygon
    {
        public int ID { get; }
        public int Width { get; }
        public int Height { get; }
        public decimal[] Center { get; }
        public decimal[,] Vertices { get; }

        public void Move(decimal centerDeltaX, decimal centerDeltaY);
        public void Rotate(int angle);
    }
}
