using Furniture;
using Vertex;

//TODO Add new method to copy data from ExtendedZone -> ZoneClass after Simulated Annealing (likely by ID property)


namespace Zones
{
    public class AnnealingZone : Zone
    {
        private Zone _parentZone;
        //ExtendedWidth and Height is dedicated to work with non integral numbers
        public decimal ExtendedWidth { get; set; }
        public decimal ExtendedHeight { get; set; }

        public AnnealingZone(Zone zone) : base(zone)
        {
            _parentZone = zone;
            ExtendedWidth = zone.Width;
            ExtendedHeight = zone.Height;
        }

        public AnnealingZone(AnnealingZone zone) : base(zone)
        {
            _parentZone = zone._parentZone;
            ExtendedWidth = zone.Width;
            ExtendedHeight = zone.Height;
        }

        public override void Resize(decimal deltaWidth, decimal deltaHeight)
        {
            ExtendedWidth += deltaWidth;
            ExtendedHeight += deltaHeight;
            Area = (double)(ExtendedWidth * ExtendedHeight);
            VertexManipulator.VertexExpanding(Vertices, deltaWidth, deltaHeight);
        }

        public Zone toZone()
        {
            _parentZone.Center = Center;
            _parentZone.Height = (int)ExtendedHeight;
            _parentZone.Width = (int)ExtendedWidth;
            _parentZone.Vertices = Vertices;

            return _parentZone;
        }

    }
}
