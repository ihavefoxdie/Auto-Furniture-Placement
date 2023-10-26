using Interfaces;
namespace GeneticAlgorithm;

public class PolygonForJson
{
    public int ID { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public decimal[]? Center { get; set; }

    public decimal[][]? JaggedVertices { get; set; }

    public string? Name { get; set; }

    public PolygonForJson(IPolygon item)
    {
        ID = item.ID;
        Width = item.Depth;
        Height = item.FrontWidth;
        Center = item.Center;
        Name = item.Name;
        JaggedVertices = new decimal[item.Vertices.GetLength(0)][];
        for (int i = 0; i < JaggedVertices.Length; i++)
        {
            JaggedVertices[i] = new decimal[item.Vertices.GetLength(1)];
            for (int j = 0; j < JaggedVertices[i].Length; j++)
            {
                JaggedVertices[i][j] = item.Vertices[i, j];
            }
        }
    }

    public PolygonForJson(int ID, int Width, int Height, decimal[] Center, decimal[][] JaggedVertices, string Name)
    {
        this.ID = ID;
        this.Width = Width;
        this.Height = Height;
        this.Center = Center;
        this.Name = Name;
        this.JaggedVertices = JaggedVertices;
    }

    public PolygonForJson()
    {

    }
}
