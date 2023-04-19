using Furniture;

//TODO Add new method to copy data from ExtendedZone -> ZoneClass after Simulated Annealing (likely by ID property)


namespace Zones
{
    public class ExtendedZone : Zone
    {
        public decimal ExtendedWidth { get; set; }
        public decimal ExtendedHeight { get; set; }

        public ExtendedZone(Zone zone) : base(zone)
        {
            ExtendedWidth = zone.Width;
            ExtendedHeight = zone.Height;
        }

        public override void Resize(decimal width, decimal height)
        {
            base.Resize(width, height);
            ExtendedWidth += width;
            ExtendedHeight += height;
            Area = (double)(ExtendedWidth * ExtendedHeight);
        }

        public Zone toZone()
        {
            Width = (int)ExtendedWidth;
            Height = (int)ExtendedHeight;

            return this;
        }

    }
}
