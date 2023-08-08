using Interfaces;

namespace Furniture
{
    public abstract class GeneralFurniture : IPolygon
    {
        #region General Properties
        readonly FurnitureData _data;
        readonly FurnitureDataFlags _flags;
        string? _parentName;


        public int ID { get { return _data.Id; } }                             //ID of the furniture object
        public int ParentID { get { return _flags.Parent; } }                  //ID of the parent furniture object
        public string Name { get { return _data.Name; } }
        public string? ParentName
        {
            get
            {
                if (_parentName == null)
                    return "empty";
                return _parentName;
            }
            private set
            {
            }
        }
        public int Rotation { get; set; }                                       //Current rotation of the object in degrees
        public int Width { get { return _data.Width; } }                        //Object width     A_____B      D_____C
        public int Height { get { return _data.Height; } }                      //Object height     D       С
                                                                                //                  |       |
                                                                                //                  |       |
                                                                                //                  A       B
        public int ClearanceWidth { get { return _data.ExtraWidth; } }          //Extra width for ClearanceArea
        public int ClearanceHeight { get { return _data.ExtraHeight; } }        //Extra height for ClearanceArea
        public string Zone { get { return _data.Zone; } }                       //String value for the zone this furniture object belongs to
        public int NearWall { get { return _flags.NearWall; } }
        public bool IgnoreWindows { get { return _flags.IgnoreWindows; } }
        public bool IsOutOfBounds { get; set; }
        public bool IsCollided { get; set; }
        #endregion


        #region Arrays of coorditantes
        public decimal[] Center { get; private set; }               //Center coords of the object
        public decimal[,] Vertices { get; private set; }            //A B C D vertices
        public decimal[,] ClearanceArea { get; private set; }       //Vertices of the extra space around the object
        #endregion


        #region Contsructors
        public GeneralFurniture(int id, string name, int length, int height, string zone, bool ignoreWindows,
                                int extraLength = 0, int extraHeight = 0, int nearWall = -1, int parent = -1,
                                bool accessible = false, string? parentName = null)
        {
            _data = new(id, name, length, height, zone, extraLength, extraHeight);
            _flags = new(ignoreWindows, nearWall, parent, accessible);
            Rotation = 0;
            Center = new decimal[2];                //Center of furniture object
            Center[0] = (decimal)Width / 2;         //X
            Center[1] = (decimal)Height / 2;        //Y
            ClearanceArea = new decimal[4, 2];      //     D_______C       where CB is front (i.e. 0 degrees rotation).
            Vertices = new decimal[4, 2];           //     |       |       If Accessible property is set to true
            ResetCoords();                          //     |       |       the front is the side that must be accessible.
                                                    //     A_______B       Accessibility is determined with pathfinding algorithm.
        }

        public GeneralFurniture(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            _data = furnitureData;
            _flags = furnitureDataFlags;
            Rotation = 0;

            Center = new decimal[2];
            Center[0] = (decimal)Width / 2;
            Center[1] = (decimal)Height / 2;

            ClearanceArea = new decimal[4, 2];
            Vertices = new decimal[4, 2];
            ResetCoords();
        }
        #endregion


////////////////////////////////////////////////////////////////////////////////////////////////////////////


        //Resetting coordinates of the rectangle for rotation and value assignment in constructor.
        public void ResetCoords()
        {
            Vertices[0, 0] = Center[0] - (decimal)Width / 2;       //A
            Vertices[0, 1] = Center[1] + (decimal)Height / 2;

            Vertices[1, 0] = Center[0] + (decimal)Width / 2;       //B
            Vertices[1, 1] = Center[1] + (decimal)Height / 2;

            Vertices[2, 0] = Center[0] + (decimal)Width / 2;       //C
            Vertices[2, 1] = Center[1] - (decimal)Height / 2;

            Vertices[3, 0] = Center[0] - (decimal)Width / 2;       //D
            Vertices[3, 1] = Center[1] - (decimal)Height / 2;


            ClearanceArea[0, 0] = Center[0] - (decimal)(Width + ClearanceWidth) / 2;
            ClearanceArea[0, 1] = Center[1] + (decimal)(Height + ClearanceHeight) / 2;

            ClearanceArea[1, 0] = Center[0] + (decimal)(Width + ClearanceWidth) / 2;
            ClearanceArea[1, 1] = Center[1] + (decimal)(Height + ClearanceHeight) / 2;

            ClearanceArea[2, 0] = Center[0] + (decimal)(Width + ClearanceWidth) / 2;
            ClearanceArea[2, 1] = Center[1] - (decimal)(Height + ClearanceHeight) / 2;

            ClearanceArea[3, 0] = Center[0] - (decimal)(Width + ClearanceWidth) / 2;
            ClearanceArea[3, 1] = Center[1] - (decimal)(Height + ClearanceHeight) / 2;
        }
    }
}