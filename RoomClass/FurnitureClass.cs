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


        #region Arrays of coorditantes
        /*  public double[] AB { get; private set; }
            public double[] BC { get; private set; }
            public double[] CD { get; private set; }
            public double[] DA { get; private set; }*/
        public double[] Center { get; private set; }
        /*public double[] A { get; private set; }
        public double[] B { get; private set; }
        public double[] C { get; private set; }
        public double[] D { get; private set; }*/
        #endregion
        public double[,] Vertices { get; private set; }

        public Furniture(int id, int length, int height, string zone, int parent = -1)
        {
            ID = id;
            ParentID = parent;
            Length = length;
            Height = height;
            Rotation = 0;
            Zone = zone;


            Center = new double[2];
            /*      AB = new double[4]; BC = new double[4];
                    CD = new double[4]; DA = new double[4];*/
            /*A = new double[2]; B = new double[2];
            C = new double[2]; D = new double[2];*/

            Vertices = new double[4, 2];
            Vertices[0, 1] = Height;
            Vertices[1, 0] = Length;    Vertices[1, 1] = Height;
            Vertices[2, 0] = Length;

            Center[0] = (double)Length / 2;     //X
            Center[1] = (double)Height / 2;     //Y

           /* A[0] = 0; A[1] = Height;
            B[0] = Length; B[1] = Height;
            C[0] = Length; C[1] = 0;
            D[0] = 0; D[1] = 0;*/

           /*      AB[0] = 0;          //X1
                    AB[1] = Height;     //Y1
                    AB[2] = Length;     //X2
                    AB[3] = Height;     //Y2


                    BC[0] = Length;     //X1
                    BC[1] = Height;     //Y1
                    BC[2] = Length;     //X2
                    BC[3] = 0;          //Y2


                    CD[0] = Length;     //X1
                    CD[1] = 0;          //Y1
                    CD[2] = 0;          //X2
                    CD[3] = 0;          //Y2


                    DA[0] = 0;          //X1
                    DA[1] = 0;          //Y1
                    DA[2] = 0;          //X2
                    DA[3] = Height;     //Y2    */
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

           /* A[0] += centerDeltaX; A[1] += centerDeltaY; B[0] += centerDeltaX; B[1] += centerDeltaY;
            C[0] += centerDeltaX; C[1] += centerDeltaY; D[0] += centerDeltaX; D[1] += centerDeltaY;*/
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
                //It will be determined whether anything will be off after constant coordinates manipulation.
            }

            /*RotatingVertex(ref A[0], ref A[1], radians); RotatingVertex(ref B[0], ref B[1], radians);
            DistanceCheck(A[0], A[1], B[0], B[1], Length); DistanceCheck(B[0], B[1], C[0], C[1], Length);

            RotatingVertex(ref C[0], ref C[1], radians); RotatingVertex(ref D[0], ref D[1], radians);
            DistanceCheck(C[0], C[1], D[0], D[1], Length); DistanceCheck(D[0], D[1], A[0], A[1], Length);*/

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
