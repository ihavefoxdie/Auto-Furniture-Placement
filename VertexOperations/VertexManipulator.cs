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

        public static void VertexReplacing(decimal[,] vertices, decimal deltaX, decimal deltaY)
        {
            for (int i = 0; i < vertices.GetLength(0); i++)
            {
                vertices[i, 0] += deltaX;
                vertices[i, 1] += deltaY;
            }
        }

    }
}
