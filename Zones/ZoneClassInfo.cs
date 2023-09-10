using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zones
{
    public static class ZoneClassInfo
    {
        public static List<double> OverlappingPenalty = new();

        public static List<double> FreeSpacePenalty = new();

        public static List<double> ZoneShapePenalty = new();

        public static List<double> SpaceRatioPenalty = new();

        public static List<double> ByWallPenalty = new();

        public static List<double> DoorSpacePenalty = new();
    }
}
