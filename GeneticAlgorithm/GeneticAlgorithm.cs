using Interfaces;
using System.Diagnostics;
using System.Text.Json;

namespace GeneticAlgorithm;

public class GeneticAlgoritm
{
    public List<IPolygonGenesContainer> Population { get; private set; }
    private bool KeepUp { get; set; }
    public int Generation { get; private set; }
    private Process visual;

    public GeneticAlgoritm(IPolygonGenesContainer container)
    {
        Population = new();
        KeepUp = true;
        for (int i = 0; i < 12; i++)
        {
            Population.Add(container);
        }
    }
    //TODO CROSSOVER AND MUTATION ALGORITHMS
    public int Start()
    {
        for (int i = 0; i < Population.Count; i++)
        {
            Population[i].PenaltyEvaluation();
        }
        int count = 0;
        while (KeepUp)
        {
            Population = Population.OrderBy(container => container.Penalty).ToList();

            int transfer = (70 * Population.Count) / 100;

            List<IPolygonGenesContainer> newContainersSet = SUS(transfer, Population);

            transfer = Population.Count - transfer;

            List<int> usedParents = new();
            for (int i = 0; i < transfer; i++)
            {
                int indexParent1;
                while (true)
                {
                    indexParent1 = new Random().Next(newContainersSet.Count);
                    if (usedParents.Contains(indexParent1))
                        continue;
                    break;
                }
                usedParents.Add(indexParent1);
                int indexParent2;
                while (true)
                {
                    indexParent2 = new Random().Next(newContainersSet.Count);
                    if (indexParent1 != indexParent2)
                        break;
                }
                newContainersSet.Add(newContainersSet[indexParent1].Crossover(newContainersSet[indexParent2]));
            }



            var newPoluation = MutatePopulations(newContainersSet);

            Population = newPoluation;
            for (int i = 0; i < Population.Count; i++)
            {
                Population[i].PenaltyEvaluation();
            }

            if (count % 5000 == 0)
            {
                SerializeBest();
                /* if (visual != null)
                 {
                     visual.CloseMainWindow();
                 }
                 visual = Process.Start("visualization\\testing shapes.exe");*/
                Console.WriteLine(count);
            }


            count++;

            if (count == 500000)
            {
                KeepUp = false;
            }
        }
        Console.WriteLine(Population[0].Penalty + " and " + Population.Last().Penalty);
        SerializeBest();
        return 1;
    }

    private void SerializeBest()
    {
        List<PolygonForJson> rectangles = new List<PolygonForJson>();

        decimal[] center = new decimal[2];
        center[0] = 15; center[1] = 15;
        decimal[][] edges = new decimal[4][];
        for (int i = 0; i < edges.Length; i++)
            edges[i] = new decimal[2];

        edges[0][0] = 0; edges[0][1] = 0;
        edges[1][0] = 30; edges[1][1] = 0;
        edges[2][0] = 30; edges[2][1] = 30;
        edges[3][0] = 0; edges[3][1] = 30;

        rectangles.Add(new PolygonForJson(1213, 30, 30, center, edges, "Room"));
        IPolygonGenesContainer contain = Population[0];
        foreach (IPolygon polygon in contain.Polygons)
            rectangles.Add(new PolygonForJson(polygon));

        string jsonFile = JsonSerializer.Serialize(rectangles);
        try
        {
            File.WriteAllText("visualization\\rectangles.json", jsonFile);
        }
        catch (Exception e)
        {
            Console.WriteLine("errro heh");
        }

    }

    private List<IPolygonGenesContainer> MutatePopulations(List<IPolygonGenesContainer> newContainersSet)
    {
        int evenOrNot = new Random().Next(2);
        for (int i = 0; i < newContainersSet.Count; i++)
        {
            if (i % 2 == evenOrNot)
            {
                MutatePopulation(newContainersSet[i]);
            }
        }
        return newContainersSet;
    }

    private void MutatePopulation(IPolygonGenesContainer container)
    {
        container.Mutate();
    }

    private static List<IPolygonGenesContainer> SUS(int amountToKeep, List<IPolygonGenesContainer> containers)
    {
        Random a = new();

        double totalPenalty = 0;
        for (int i = 0; i < containers.Count; i++)
            totalPenalty += containers[i].Penalty;

        int distance = (int)(totalPenalty / amountToKeep);

        List<int> pointers = new();
        for (int i = 0; i < amountToKeep; i++)
            pointers.Add(a.Next(distance) + i * distance);

        return RWS(containers, pointers);
    }

    private static List<IPolygonGenesContainer> RWS(List<IPolygonGenesContainer> containers, List<int> pointers)
    {
        List<IPolygonGenesContainer> containersToKeep = new();

        for (int i = 0; i < pointers.Count; i++)
        {
            int selector = -1;
            double sum = 0;
            while (sum <= pointers[i])
            {
                selector++;
                sum += containers[selector].Penalty;
            }
            containersToKeep.Add(containers[selector]);
        }

        return containersToKeep;
    }
}
