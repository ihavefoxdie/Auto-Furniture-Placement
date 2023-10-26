using Furniture;
using Vertex;

//TODO Add new method to copy data from ExtendedZone -> ZoneClass after Simulated Annealing (likely by ID property)


namespace Zones
{
    public class AnnealingZone : Zone
    {
        private Zone _parentZone;
        //ExtendedWidth and FrontWidth is dedicated to work with non integral numbers
        public decimal ExtendedWidth { get; set; }
        public decimal ExtendedHeight { get; set; }

        public AnnealingZone(Zone zone) : base(zone)
        {
            _parentZone = zone;
            ExtendedWidth = zone.Depth;
            ExtendedHeight = zone.FrontWidth;
        }

        public AnnealingZone(AnnealingZone zone) : base(zone)
        {
            _parentZone = zone._parentZone;
            ExtendedWidth = zone.Depth;
            ExtendedHeight = zone.FrontWidth;
        }

        public override void Resize(decimal deltaWidth, decimal deltaHeight)
        {
            if (ExtendedWidth + deltaWidth <= 0 || ExtendedHeight + deltaHeight <= 0)
                return;
            else
            {
                ExtendedWidth += deltaWidth;



                if (ExtendedHeight + deltaHeight > 0)
                    ExtendedHeight += deltaHeight;

                Area = (double)(ExtendedWidth * ExtendedHeight);
                VertexManipulator.VertexResetting(Vertices, Center, (int)ExtendedWidth, (int)ExtendedHeight);
            }
        }

        public Zone toZone()
        {
            _parentZone.Center = Center;
            _parentZone.FrontWidth = (int)ExtendedHeight;
            _parentZone.Depth = (int)ExtendedWidth;
            _parentZone.Area = Area;
            VertexManipulator.VertexResetting(_parentZone.Vertices, _parentZone.Center, _parentZone.Depth, _parentZone.FrontWidth);
            return _parentZone;
        }

    }
}
