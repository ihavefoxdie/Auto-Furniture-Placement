namespace Interfaces
{
    public interface IPolygonContainer
    {
        public void PenaltyEvaluation();
        public double Penalty { get; set; }
        public int ContainerHeight { get; }
        public int ContainerWidth { get; }
        public List<IPolygon> Polygons { get; }
    }
}
