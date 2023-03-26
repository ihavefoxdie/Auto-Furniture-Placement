using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomClass
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
    }
}
