namespace RoomClass
{
    public static class Rasterization
    {
        public static void Print(int[,] space)
        {
            for (int i = 0; i < space.GetLength(0); i++)
            {
                for (int j = 0; j < space.GetLength(1); j++)
                {
                    if (space[i, j] != 0)
                        Console.Write("7  ");
                    else
                        Console.Write(".  ");
                }
                Console.WriteLine();
            }
        }

        public static void Line(int x, int y, int x2, int y2, int[,] space, int color)
        {
            //travel distance
            int dx = Math.Abs(x2 - x);          //distance between x2 and x1
            int dy = Math.Abs(y2 - y);          //distance between y2 and y1

            //cursor direction determination
            int ix = -1;
            if (x < x2) ix = 1;                 

            int iy = -1;
            if (y < y2) iy = 1;

            int e = 0;

            for (int i = 0; i < dx + dy; i++)
            {
                space[x, y] = color;                //cursor
                int e1 = e + dy;
                int e2 = e - dx;

                //cursor trajectory determination
                if (Math.Abs(e1) < Math.Abs(e2))    
                {
                    x += ix;
                    e = e1;
                }
                else
                {
                    y += iy;
                    e = e2;
                }
                Console.Clear();
                Print(space);
            }
        }
    }
}
