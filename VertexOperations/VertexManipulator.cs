using Interfaces;

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

        public static bool DeterminRectangleCollision(IPolygon polygon1, IPolygon polygon2)
        {
            if (
                polygon1.Center[0] - polygon1.Width / 2 < polygon2.Center[0] + polygon2.Width / 2 &&
                polygon1.Center[0] + polygon1.Width / 2 > polygon2.Center[0] - polygon2.Width / 2 &&
                polygon1.Center[1] - polygon1.Height / 2 < polygon2.Center[1] + polygon2.Height / 2 &&
                polygon1.Height / 2 + polygon1.Center[1] > polygon2.Center[1] - polygon2.Height / 2
              )
                return true;
            return false;
        }

        public static void VertexExpanding(decimal[,] vertices, decimal deltaX, decimal deltaY)
        {
            vertices[2, 0] -= deltaX;
            vertices[2, 1] += deltaY;

            vertices[3, 0] += deltaX;
            vertices[3, 1] += deltaY;

            vertices[0, 0] += deltaX;
            vertices[0, 1] -= deltaY;

            vertices[1, 0] -= deltaX;
            vertices[1, 1] -= deltaY;
        }

        public static void VertexExpanding(decimal[,] vertices, int delta)
        {
            vertices[2, 0] -= delta;
            vertices[2, 1] += delta;

            vertices[3, 0] += delta;
            vertices[3, 1] += delta;

            vertices[0, 0] += delta;
            vertices[0, 1] -= delta;

            vertices[1, 0] -= delta;
            vertices[1, 1] -= delta;
        }


        public static void VertexResetting(decimal[,] vertices, decimal[] Center, int Width, int Height)
        {

            vertices[2, 0] = Center[0] - (decimal)Width / 2;       //A
            vertices[2, 1] = Center[1] + (decimal)Height / 2;

            vertices[3, 0] = Center[0] + (decimal)Width / 2;       //B
            vertices[3, 1] = Center[1] + (decimal)Height / 2;

            vertices[0, 0] = Center[0] + (decimal)Width / 2;       //C
            vertices[0, 1] = Center[1] - (decimal)Height / 2;

            vertices[1, 0] = Center[0] - (decimal)Width / 2;       //D
            vertices[1, 1] = Center[1] - (decimal)Height / 2;
        }

        public static bool IsPolygonInsideRoom(IPolygon zone, int roomWidth, int roomHeight)
        {
            for (int i = 0; i < 4; i++)
            {
                if (!IsVertexInsideRoom(zone.Vertices[i, 0], zone.Vertices[i, 1], roomWidth, roomHeight))
                    return false;
            }
            return true;
        }

        private static bool IsVertexInsideRoom(decimal x, decimal y, int roomWidth, int roomHeight)
        {
            if (x > roomWidth || x < 0)
                return false;

            if (y > roomHeight || y < 0)
                return false;

            return true;
        }

    }
}
