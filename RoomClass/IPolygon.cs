namespace RoomClass
{
    internal interface IPolygon
    {
        public int Length { get; }
        public int Height { get; }
        public decimal[] Center { get; }
        public decimal[,] Vertices { get; }

        public void Move(decimal centerDeltaX, decimal centerDeltaY);
    }
}
