using Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Furniture
{
    public abstract class GeneralFurniture : IPolygon, ICloneable
    {
        #region General Properties
        public FurnitureData Data { get; set; }
        public FurnitureDataFlags Flags { get; set; }


        public string Name { get { return Data.Name; } }
        public int ID { get { return Data.ID; } }                              //ID of the furniture object
        public int Rotation { get; set; }                                      //Current rotation of the object in degrees
        public int Depth { get { return Data.Depth; } }                        //Object width       A_______B      D_______C
        public int FrontWidth { get { return Data.FrontWidth; } }              //Object front width D       С
                                                                               //                   |       |
                                                                               //                   |       |
                                                                               //                   A       B
        public bool IsOutOfBounds { get; set; }
        public bool IsCollided { get; set; }
        #endregion


        #region Arrays of coorditantes
        public decimal[] Center { get; private set; }               //Center coords of the object
        public decimal[,] Vertices { get; private set; }            //A B C D vertices
        public decimal[,] ClearanceArea { get; private set; }       //Vertices of the extra space around the object
        #endregion


        #region Contsructors
        public GeneralFurniture(int id, string name, int depth, int frontWidth, string zone, bool ignoreWindows,
                                int extraDepth = 0, int extraWidth = 0, int nearWall = -1, bool parent = false,
                                bool accessible = false)
        {
            Data = new(id, name, depth, frontWidth, zone, extraDepth, extraWidth);
            Flags = new(ignoreWindows, nearWall, parent, accessible);
            Rotation = 0;
            Center = new decimal[2];                //Center of furniture object
            Center[0] = (decimal)Depth / 2;         //     X
            Center[1] = (decimal)FrontWidth / 2;    //     Y

            ClearanceArea = new decimal[4, 2];      //          D_______C       where CB is front (i.e. 0 degrees rotation).
            Vertices = new decimal[4, 2];           //          |       |       If Accessible property is set to true
            ResetCoords();                          //          |       |       the front is the side that must be accessible.
                                                    //          A_______B       Accessibility is determined with pathfinding algorithm.
        }

        public GeneralFurniture(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            Data = furnitureData;
            Flags = furnitureDataFlags;
            Rotation = 0;

            Center = new decimal[2];
            Center[0] = (decimal)Depth / 2;
            Center[1] = (decimal)FrontWidth / 2;

            ClearanceArea = new decimal[4, 2];
            Vertices = new decimal[4, 2];
            ResetCoords();
        }
        #endregion


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


        //Resetting coordinates of the rectangle for rotation and value assignment in constructor.
        public void ResetCoords()
        {
            Vertices[0, 0] = Center[0] - (decimal)Depth / 2;       //A
            Vertices[0, 1] = Center[1] + (decimal)FrontWidth / 2;

            Vertices[1, 0] = Center[0] + (decimal)Depth / 2;       //B
            Vertices[1, 1] = Center[1] + (decimal)FrontWidth / 2;

            Vertices[2, 0] = Center[0] + (decimal)Depth / 2;       //C
            Vertices[2, 1] = Center[1] - (decimal)FrontWidth / 2;

            Vertices[3, 0] = Center[0] - (decimal)Depth / 2;       //D
            Vertices[3, 1] = Center[1] - (decimal)FrontWidth / 2;


            ClearanceArea[0, 0] = Center[0] - (decimal)(Depth + Data.ExtraDepth) / 2;
            ClearanceArea[0, 1] = Center[1] + (decimal)(FrontWidth + Data.ExtraWidth) / 2;

            ClearanceArea[1, 0] = Center[0] + (decimal)(Depth + Data.ExtraDepth) / 2;
            ClearanceArea[1, 1] = Center[1] + (decimal)(FrontWidth + Data.ExtraWidth) / 2;

            ClearanceArea[2, 0] = Center[0] + (decimal)(Depth + Data.ExtraDepth) / 2;
            ClearanceArea[2, 1] = Center[1] - (decimal)(FrontWidth + Data.ExtraWidth) / 2;

            ClearanceArea[3, 0] = Center[0] - (decimal)(Depth + Data.ExtraDepth) / 2;
            ClearanceArea[3, 1] = Center[1] - (decimal)(FrontWidth + Data.ExtraWidth) / 2;
        }

        public object Clone()
        {
            GeneralFurniture obj = (GeneralFurniture)MemberwiseClone();
            obj.Center = Center.ToArray();

            obj.Vertices = new decimal[Vertices.GetLength(0),Vertices.GetLength(1)];
            for (int i = 0; i < obj.Vertices.GetLength(0); i++)
            {
                for (int j = 0; j < obj.Vertices.GetLength(1); j++)
                {
                    obj.Vertices[i, j] = Vertices[i, j];
                }
            }

            obj.ClearanceArea = new decimal[ClearanceArea.GetLength(0), ClearanceArea.GetLength(1)];
            for (int i = 0; i < obj.ClearanceArea.GetLength(0); i++)
            {
                for (int j = 0; j < obj.ClearanceArea.GetLength(1); j++)
                {
                    obj.ClearanceArea[i, j] = ClearanceArea[i, j];
                }
            }

            obj.Data = (FurnitureData)Data.Clone();
            obj.Flags = (FurnitureDataFlags)Flags.Clone();

            return obj;
        }
    }
}