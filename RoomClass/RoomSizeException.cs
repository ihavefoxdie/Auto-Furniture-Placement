using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomProject
{
    internal class RoomSizeException : Exception
    {
        public RoomSizeException(string? message, int width, int height) : base(message)
        {
            if (width <=0) message += $" | Incorrect Width : {width}";
            if (height <= 0) message += $" | Incorrect Height : {height}";
            Debug.Indent();
            Debug.Write(message);
            Debug.Unindent();
        }
    }
}
