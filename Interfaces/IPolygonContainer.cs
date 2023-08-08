namespace Interfaces
{
    public interface IPolygonContainer
    {
        public void PenaltyEvaluation();
        public double Penalty { get; set; }
        public List<IPolygon> Polygons { get; }
    }
}
