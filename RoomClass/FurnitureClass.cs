

namespace RoomClass
{
    public class Furniture : IPolygon
    {
        public int ID { get; private set; }
        public int ParentID { get; set; }

        public int Rotation { get; private set; }
        public int Length { get; private set; }
        public int Height { get; private set; }
        public string Zone { get; private set; }
        public bool IgnoreWindows { get; private set; }
        public int NearWall { get; private set; }

        public bool IsOutOfBounds { get; set; }
        public bool IsCollided { get; set; }


        #region Arrays of coorditantes
        public decimal[] Center { get; private set; }
        public decimal[,] Vertices { get; private set; }
        #endregion


        public Furniture(int id, int length, int height, string zone, bool ignoreWindows, int nearWall = -1, int parent = -1)
        {
            ID = id;
            ParentID = parent;
            Length = length;
            Height = height;
            Rotation = 0;
            Zone = zone;
            IgnoreWindows = ignoreWindows;
            NearWall = nearWall;

            Center = new decimal[2];
            Center[0] = (decimal)Length / 2;     //X
            Center[1] = (decimal)Height / 2;     //Y

            Vertices = new decimal[4, 2];
            Vertices[0, 1] = Height;                            //A
            Vertices[1, 0] = Length; Vertices[1, 1] = Height;   //B
            Vertices[2, 0] = Length;                            //C
                                                                //D
        }


        #region Moving Furniture
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


        /*private static bool DistanceCheck(double x1, double y1, double x2, double y2, int length)
        {
            //There is a risk of double values not aligning perfectly with the int value of length, hence the precausion
            double errorMarginLength = (double)length;

            double distanceBetween = Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));

            if (distanceBetween > errorMarginLength + 0.05 || distanceBetween < errorMarginLength - 0.05)
                return false;

            return true;
        }*/
        #endregion


        #region Rotation

        public void Rotate(int angle)
        {
            ResetCoords();

            Rotation += angle;
            while (Rotation >= 360)
                Rotation -= 360;
            while (Rotation < 0)
                Rotation += 360;

            double radians = Rotation * (Math.PI / 180);

            for (int i = 0; i < Vertices.GetLength(0); i++)
            {
                RotatingVertex(ref Vertices[i, 0], ref Vertices[i, 1], radians);
            }

        }

        private void ResetCoords()
        {
            Vertices[0, 0] = Center[0] - (decimal)Length / 2;
            Vertices[0, 1] = Center[1] + (decimal)Height / 2;

            Vertices[1, 0] = Center[0] + (decimal)Length / 2;
            Vertices[1, 1] = Center[1] + (decimal)Height / 2;

            Vertices[2, 0] = Center[0] + (decimal)Length / 2;
            Vertices[2, 1] = Center[1] - (decimal)Height / 2;

            Vertices[3, 0] = Center[0] - (decimal)Length / 2;
            Vertices[3, 1] = Center[1] - (decimal)Height / 2;
        }

        private void RotatingVertex(ref decimal x, ref decimal y, double radians)
        {
            // Translation point to the origin
            decimal tempX = x - Center[0];
            decimal tempY = y - Center[1];

            // Rotation application
            decimal rotatedX = tempX * (decimal)Math.Cos(radians) - tempY * (decimal)Math.Sin(radians);
            decimal rotatedY = tempX * (decimal)Math.Sin(radians) + tempY * (decimal)Math.Cos(radians);

            // Translating back
            x = rotatedX + Center[0];
            y = rotatedY + Center[1];
        }

        public object Clone()
        {
            Furniture item = new Furniture(ID, Height, Length, Zone, IgnoreWindows, NearWall, ParentID);
            item.Center = (decimal[])this.Center.Clone();
            item.Vertices = (decimal[,])this.Vertices.Clone();
            item.Rotation = this.Rotation;
            return item;
        }
        #endregion
    }
}