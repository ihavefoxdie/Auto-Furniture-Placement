using Furniture;
using Vertex;

//TODO Add new method to copy data from ExtendedZone -> ZoneClass after Simulated Annealing (likely by ID property)


namespace Zones
{
    public class AnnealingZone : Zone
    {
        public decimal ExtendedWidth { get; set; }
        public decimal ExtendedHeight { get; set; }

        public AnnealingZone(Zone zone, int aisle) : base(zone)
        {
            ExtendedWidth = zone.Width;
            ExtendedHeight = zone.Height;
        }

        public override void Resize(decimal width, decimal height)
        {
            ExtendedWidth += width;
            ExtendedHeight += height;
            Area = (double)(ExtendedWidth * ExtendedHeight);
            VertexManipulator.VertexExpanding(Vertices, width, height);
        }

        public Zone toZone()
        {

            throw new NotImplementedException();
        }

    }
}
