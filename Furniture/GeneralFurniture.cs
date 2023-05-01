using Interfaces;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace Furniture
{
    public abstract class GeneralFurniture : IPolygon
    {
        #region General Properties
        public int ID { get; protected set; }                 //ID of the furniture object
        public int ParentID { get; private set; }           //ID of the parent furniture object
        public string Name { get; protected set; }
        private string? _parentName;
        public string? ParentName
        {
            get
            {
                if(_parentName == null)
                    return "empty";
                return _parentName;
            }
            private set
            {
            }
        }
        public int Rotation { get; private set; }           //Current rotation of the object in degrees
        public int Width { get; protected set; }              //Object width     A_____B      D_____C
        public int Height { get; protected set; }             //Object height     D       С
                                                            //                  |       |
                                                            //                  |       |
                                                            //                  A       B
        public int ClearanceWidth { get; private set; }         //Extra width for ClearanceArea
        public int ClearanceHeight { get; private set; }        //Extra height for ClearanceArea
        public string Zone { get; protected set; }            //String value for the zone this furniture object belongs to
        #endregion


        #region Flags
        public bool IgnoreWindows { get; protected set; }     //Determines whether the furniture object can be placed in front of a window
        public int NearWall { get; protected set; }           //Determines whether the furniture object must be placed near wall and the distance between the two
        public bool Accessible { get; protected set; }        //Determines whether the furniture object must be accessible

        public bool IsOutOfBounds { get; set; }             //Is the furniture object currently out of bounds
        public bool IsCollided { get; set; }                //Is the furniture object currently collided with another
        #endregion


        #region Rotation Delegate
        public delegate void VertexRotation(ref decimal x, ref decimal y, double radians, int centerX, int centerY);
        public VertexRotation? RotateVertex { get; set; }
        #endregion


        #region Arrays of coorditantes
        public decimal[] Center { get; private set; }               //Center coords of the object
        public decimal[,] Vertices { get; private set; }            //A B C D vertices
        public decimal[,] ClearanceArea { get; private set; }       //Vertices of the extra space around the object
        #endregion


        #region Contsructor
        public GeneralFurniture(int id, string name, int length, int height, string zone, bool ignoreWindows,
                                int extraLength = 0, int extraHeight = 0, int nearWall = -1, int parent = -1,
                                bool accessable = false, string? parentName = null)
        {
            ID = id;
            ParentID = parent;
            Name = name;
            Width = length;
            Height = height;
            Rotation = 0;
            Zone = zone;
            IgnoreWindows = ignoreWindows;
            ClearanceWidth = extraLength;               //Extra width applied to the base width for clearance area boundries
            ClearanceHeight = extraHeight;              //Same as ClearanceWidth but applied for height
            NearWall = nearWall;                    //Maximum distance allowed between a wall and the furniture object (-1 is set to ignore this charactiristic)
            Accessible = accessable;

            Center = new decimal[2];                //Center of furniture object
            Center[0] = (decimal)Width / 2;         //X
            Center[1] = (decimal)Height / 2;        //Y

            ClearanceArea = new decimal[4, 2];      //     D_______C       where CB is front (i.e. 0 degrees rotation).
            Vertices = new decimal[4, 2];           //     |       |       If Accessible property is set to true
            ResetCoords();                          //     |       |       the front is the side that must be accessable.
                                                    //     A_______B       Accessibility is determined with pathfinding algorithm.
        }

        public GeneralFurniture(FurnitureData furnitureData, FurnitureDataFlags furnitureDataFlags)
        {
            string? parentName = null;

            ID = furnitureData.Id;
            ParentID = furnitureDataFlags.Parent;
            Name = furnitureData.Name;
            Width = furnitureData.Length;
            Height = furnitureData.Height;
            Rotation = 0;
            Zone = furnitureData.Zone;
            IgnoreWindows = furnitureDataFlags.IgnoreWindows;
            ClearanceWidth = furnitureData.ExtraLength;     
            ClearanceHeight = furnitureData.ExtraHeight;    
            NearWall = furnitureDataFlags.NearWall;         
            Accessible = furnitureDataFlags.Accessible;

            Center = new decimal[2];              
            Center[0] = (decimal)Width / 2;       
            Center[1] = (decimal)Height / 2;      

            ClearanceArea = new decimal[4, 2];    
            Vertices = new decimal[4, 2];         
            ResetCoords();                                                                          
        }


        #endregion




        #region Moving Furniture
        public virtual void Move(decimal centerDeltaX, decimal centerDeltaY)
        {
            Center[0] += centerDeltaX;
            Center[1] += centerDeltaY;

            for (int i = 0; i < Vertices.GetLength(0); i++)
            {
                Vertices[i, 0] += centerDeltaX;
                Vertices[i, 1] += centerDeltaY;
            }
        }
        #endregion


        #region Rotation
        public void Rotate(int angle)
        {
            if (RotateVertex == null)
                return;

            ResetCoords();

            Rotation += angle;
            while (Rotation >= 360)
                Rotation -= 360;
            while (Rotation < 0)
                Rotation += 360;

            double radians = Rotation * (Math.PI / 180);

            for (int i = 0; i < Vertices.GetLength(0); i++)
            {
                RotateVertex(ref Vertices[i, 0], ref Vertices[i, 1], radians, (int)Center[0], (int)Center[1]);
            }
        }

        //Resetting coordinates of the rectangle for rotation and value assignment in constructor.
        private void ResetCoords()
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


        /*public object Clone()
        {
            Furniture item = new(ID, Height, Width, Zone, IgnoreWindows, NearWall, ParentID)
            {
                Center = (decimal[])this.Center.Clone(),
                Vertices = (decimal[,])this.Vertices.Clone(),
                Rotation = this.Rotation
            };
            return item;
        }*/
        #endregion
    }
}