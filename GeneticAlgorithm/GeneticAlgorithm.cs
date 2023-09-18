using Interfaces;
using System.Text.Json;

namespace GeneticAlgorithm;

public class GeneticAlgoritm
{
    public List<IPolygonGenesContainer> Population { get; private set; }
    private bool KeepUp { get; set; }
    public int Generation { get; private set; }
    //private Process visual;

    //TODO: thorough debug
    //TODO: determine why SOMETIMES AT THE START out of bounds exception is being thrown
    public GeneticAlgoritm(IPolygonGenesContainer container)
    {
        List<double> fuckmyass = new();
        Population = new();
        KeepUp = true;
        for (int i = 0; i < 12; i++)
        {
            Population.Add((IPolygonGenesContainer)container.Clone());
            Population[i].PenaltyEvaluation();
        }

        for (int i = 0; i <= 12; i++)
        {
            for (int j = 0; j < 12; j++)
            {
                Population[j].PenaltyEvaluation();
                Console.WriteLine(j + " - " + Population[j].Penalty);
                if (j < fuckmyass.Count)
                    if (fuckmyass[j] != Population[j].Penalty)
                        Console.WriteLine("FUCK!!!!!!");
            }
            if (i == 12)
                break;
            Console.WriteLine("\n");
            Population[i].Randomize();
            Population[i].PenaltyEvaluation();
            fuckmyass.Add(Population[i].Penalty);
        }

        fuckmyass.Clear();
        for (int j = 0; j < 12; j++)
        {
            Population[j].PenaltyEvaluation();
            fuckmyass.Add(Population[j].Penalty);
            Console.WriteLine(j + " - " + Population[j].Penalty);
        }

        for (int i = 0; i < 500; i++)
        {
            Console.WriteLine("iteration for evaluation: " + i);
            for (int j = 0; j < 12; j++)
            {
                Population[j].PenaltyEvaluation();
                if (Population[j].Penalty != fuckmyass[j])
                    Console.WriteLine("FUCK MY ASS");
                Console.WriteLine(j + " - " + Population[j].Penalty);
            }
            Console.WriteLine("\n");
        }
    }

    public int Start()
    {
        int count = 0;
        while (KeepUp)
        {
            Population = Population.OrderBy(container => container.Penalty).ToList();

            if (count % 1000 == 0)
            {
                SerializeElement(0);
                /* if (visual != null)
                 {
                     visual.CloseMainWindow();
                 }
                 visual = Process.Start("visualization\\testing shapes.exe");*/
                Console.WriteLine(count);
            }

            if (count >= 500000 && Population[0].Penalty < 10)
            {
                KeepUp = false;
            }

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
                newContainersSet.Add(newContainersSet[indexParent1].Crossover((IPolygonGenesContainer)newContainersSet[indexParent2].Clone()));
            }



            var newPoluation = MutatePopulations(newContainersSet);

            Population = newPoluation;
            for (int i = 0; i < Population.Count; i++)
            {
                Population[i].PenaltyEvaluation();
            }




            count++;
        }
        Console.WriteLine(Population[0].Penalty + " and " + Population.Last().Penalty);
        SerializeElement(0);
        return 1;
    }

    private void SerializeElement(int n)
    {
        List<PolygonForJson> rectangles = new();

        decimal[] center = new decimal[2];
        center[0] = Population[n].ContainerWidth / 2; center[1] = Population[n].ContainerHeight / 2;
        decimal[][] edges = new decimal[4][];

        for (int i = 0; i < edges.Length; i++)
            edges[i] = new decimal[2];

        edges[0][0] = 0; edges[0][1] = 0;
        edges[1][0] = Population[n].ContainerWidth; edges[1][1] = 0;
        edges[2][0] = Population[n].ContainerWidth; edges[2][1] = Population[n].ContainerHeight;
        edges[3][0] = 0; edges[3][1] = Population[n].ContainerHeight;

        rectangles.Add(new PolygonForJson(1213, Population[n].ContainerWidth, Population[n].ContainerHeight, center, edges, ""));
        IPolygonGenesContainer contain = Population[n];

        foreach (IPolygon polygon in contain.Polygons)
            rectangles.Add(new PolygonForJson(polygon));

        string jsonFile = JsonSerializer.Serialize(rectangles);
        try
        {
            File.WriteAllText("visualization\\rectangles.json", jsonFile);
        }
        catch
        { }
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
                sum += containers[selector].Penalty; // this motherfucker is throwing the exception!
            }
            containersToKeep.Add(containers[containers.Count - selector - 1]);
        }

        return containersToKeep;
    }
}
