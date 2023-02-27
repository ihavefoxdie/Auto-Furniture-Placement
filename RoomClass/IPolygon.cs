namespace RoomClass
{
    internal interface IPolygon
    {
        public int Length { get; }
        public int Height { get; }
        public double[] Center { get; }
        public double[,] Vertices { get; }

        public void Move(double centerDeltaX, double centerDeltaY);
    }
}
