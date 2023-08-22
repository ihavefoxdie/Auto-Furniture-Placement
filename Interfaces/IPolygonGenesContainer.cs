namespace Interfaces
{
    public interface IPolygonGenesContainer
    {
        public void PenaltyEvaluation();
        public double Penalty { get; set; }
        public int ContainerHeight { get; }
        public int ContainerWidth { get; }
        public List<IPolygon> Polygons { get; }
        public void Mutate();
        public IPolygonGenesContainer Crossover(IPolygonGenesContainer parent);
    }
}
