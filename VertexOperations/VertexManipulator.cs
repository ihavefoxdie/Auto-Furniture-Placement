namespace Vertex
{
    public static class VertexManipulator
    {
        public static void VertexRotation(ref decimal x, ref decimal y, double radians, int centerX, int centerY)
        {
            // Translation point to the origin
            decimal tempX = x - centerX;
            decimal tempY = y - centerY;

            // Rotation application
            decimal rotatedX = tempX * (decimal)Math.Cos(radians) - tempY * (decimal)Math.Sin(radians);
            decimal rotatedY = tempX * (decimal)Math.Sin(radians) + tempY * (decimal)Math.Cos(radians);

            // Translating back
            x = rotatedX + centerX;
            y = rotatedY + centerY;
        }

        public static bool DetermineCollision(decimal[,] vertices, decimal[] point)
        {
            bool collision = false;

            for (int i = 0, j = vertices.GetLength(0) - 1; i < vertices.GetLength(0); j = i++)
            {
                if (((vertices[i, 1] <= point[1] && point[1] < vertices[j, 1]) ||
                    (vertices[j, 1] <= point[1] && point[1] < vertices[i, 1])) &&
                    point[0] < (vertices[j, 0] - vertices[i, 0]) * (point[1] - vertices[i, 1]) /
                    (vertices[j, 1] - vertices[i, 1]) + vertices[i, 0])
                    collision = !collision;
            }
            return collision;
        }

        public static void VertexExpanding(decimal[,] vertices, decimal deltaX, decimal deltaY)
        {
            vertices[0, 0] -= deltaX;
            vertices[0, 1] -= deltaY;

            vertices[1, 0] += deltaX;
            vertices[1, 1] -= deltaY;

            vertices[2, 0] += deltaX;
            vertices[2, 1] += deltaY;

            vertices[3, 0] -= deltaX;
            vertices[3, 1] += deltaY;
        }

        public static void VertexResetting(decimal[,] vertices, decimal[] Center, int Width, int Height)
        {

            vertices[0, 0] = Center[0] - (decimal)Width / 2;       //A
            vertices[0, 1] = Center[1] + (decimal)Height / 2;

            vertices[1, 0] = Center[0] + (decimal)Width / 2;       //B
            vertices[1, 1] = Center[1] + (decimal)Height / 2;

            vertices[2, 0] = Center[0] + (decimal)Width / 2;       //C
            vertices[2, 1] = Center[1] - (decimal)Height / 2;

            vertices[3, 0] = Center[0] - (decimal)Width / 2;       //D
            vertices[3, 1] = Center[1] - (decimal)Height / 2;
        }

        public static void VertexResetting(decimal[,] vertices, decimal[] Center, decimal Width, decimal Height)
        {

            vertices[0, 0] = Center[0] - Width / 2;       //A
            vertices[0, 1] = Center[1] + Height / 2;

            vertices[1, 0] = Center[0] + Width / 2;       //B
            vertices[1, 1] = Center[1] + Height / 2;

            vertices[2, 0] = Center[0] + Width / 2;       //C
            vertices[2, 1] = Center[1] - Height / 2;

            vertices[3, 0] = Center[0] - Width / 2;       //D
            vertices[3, 1] = Center[1] - Height / 2;
        }

    }
}
