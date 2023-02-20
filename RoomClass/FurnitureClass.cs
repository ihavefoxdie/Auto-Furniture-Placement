namespace RoomClass
{
    public class Furniture
    {
        public int ID { get; private set; }
        public int ParentID { get; set; }

        public double Rotation { get; private set; }
        public int Length { get; private set; }
        public int Height { get; private set; }
        public string Zone { get; private set; }

        public bool IsOutOfBounds { get; set; }
        public bool IsCollided { get; set; }


        #region Arrays of coorditantes
        public double[] Center { get; private set; }
        public double[,] Vertices { get; private set; }
        #endregion


        public Furniture(int id, int length, int height, string zone, int parent = -1)
        {
            ID = id;
            ParentID = parent;
            Length = length;
            Height = height;
            Rotation = 0;
            Zone = zone;


            Center = new double[2];
            Center[0] = (double)Length / 2;     //X
            Center[1] = (double)Height / 2;     //Y

            Vertices = new double[4, 2];
            Vertices[0, 1] = Height;
            Vertices[1, 0] = Length;    Vertices[1, 1] = Height;
            Vertices[2, 0] = Length;
        }


        #region Moving Furniture
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
        public void Rotate(double angle)
        {
            double radians = angle * (Math.PI / 180);

            for (int i = 0; i < Vertices.GetLength(0); i++)
            {
                RotatingVertex(ref Vertices[i, 0], ref Vertices[i, 1], radians);
            }
            Rotation = angle;
        }

        private void RotatingVertex(ref double x, ref double y, double radians)
        {
            // Translation point to the origin
            double tempX = x - Center[0];
            double tempY = y - Center[1];

            // Rotation application
            double rotatedX = tempX * Math.Cos(radians) - tempY * Math.Sin(radians);
            double rotatedY = tempX * Math.Sin(radians) + tempY * Math.Cos(radians);

            // Translating back
            x = rotatedX + Center[0];
            y = rotatedY + Center[1];
        }
        #endregion
    }
}
