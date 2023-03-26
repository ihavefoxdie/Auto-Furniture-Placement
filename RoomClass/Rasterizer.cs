using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomClass
{
    public static class Rasterizer
    {
        public delegate void Raster(int x1, int y1, int x2, int y2, int[,] space, int color);
        public static Raster? Rasterize { get; set; }

        public static int[,] Rasterization(List<IPolygon> polygons, int containerWidth, int containerHeight)
        {
            if (polygons == null) throw new NullReferenceException("The list containing polygons is null!");

            int[,] RoomArray;

            if (Rasterize == null)
            {
                throw new NullReferenceException("The Rasterize delegate has not been assigned!");
            }

            int minimalVal = 0;
            for (int i = 0; i < polygons.Count; i++)
            {
                for (int j = 0; j < polygons[i].Vertices.GetLength(0); j++)
                {
                    for (int k = 0; k < polygons[i].Vertices.GetLength(1); k++)
                    {
                        if ((int)polygons[i].Vertices[j, k] < 0 && (int)polygons[i].Vertices[j, k] < minimalVal)
                            minimalVal = (int)polygons[i].Vertices[j, k];
                    }
                }
            }
            minimalVal = Math.Abs(minimalVal);


            int maxVal = 0;
            for (int i = 0; i < polygons.Count; i++)
            {
                for (int j = 0; j < polygons[i].Vertices.GetLength(0); j++)
                {
                    for (int k = 0; k < polygons[i].Vertices.GetLength(1); k++)
                    {
                        if (((int)polygons[i].Vertices[j, k] >= containerWidth || (int)polygons[i].Vertices[j, k] >= containerHeight) && (int)polygons[i].Vertices[j, k] > maxVal)
                            maxVal = (int)polygons[i].Vertices[j, k] + 1;
                    }
                }
            }


            if (maxVal > containerWidth)
                maxVal -= containerWidth;
            else if (maxVal > containerHeight)
                maxVal -= containerHeight;

            int addVal = minimalVal + maxVal;
            int spaceWidth = containerWidth + addVal; int spaceHeight = containerHeight + addVal;
            maxVal = Math.Max(spaceWidth, spaceHeight);
            RoomArray = new int[maxVal, maxVal];


            for (int i = 0; i < polygons.Count; i++)
            {
                for (int j = 0; j < polygons[i].Vertices.GetLength(0); j++)
                {
                    if (j < polygons[i].Vertices.GetLength(0) - 1)
                        Rasterize((int)polygons[i].Vertices[j, 0] + minimalVal, (int)polygons[i].Vertices[j, 1] + minimalVal, (int)polygons[i].Vertices[j + 1, 0] + minimalVal, (int)polygons[i].Vertices[j + 1, 1] + minimalVal, RoomArray, polygons[i].ID);
                    else
                        Rasterize((int)polygons[i].Vertices[j, 0] + minimalVal, (int)polygons[i].Vertices[j, 1] + minimalVal, (int)polygons[i].Vertices[0, 0] + minimalVal, (int)polygons[i].Vertices[0, 1] + minimalVal, RoomArray, polygons[i].ID);
                }
            }


            Rasterize(minimalVal, minimalVal, containerWidth + minimalVal, minimalVal, RoomArray, -1);
            Rasterize(containerWidth + minimalVal - 1, minimalVal, containerWidth + minimalVal - 1, containerHeight + minimalVal - 1, RoomArray, -1);
            Rasterize(containerWidth + minimalVal - 1, containerHeight + minimalVal - 1, minimalVal, containerHeight + minimalVal - 1, RoomArray, -1);
            Rasterize(minimalVal, containerHeight + minimalVal - 1, minimalVal, minimalVal, RoomArray, -1);

            return RoomArray;
        }
    }
}
