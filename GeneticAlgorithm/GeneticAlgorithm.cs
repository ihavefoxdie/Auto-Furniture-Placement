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
        int size = 8;
        for (int i = 0; i < size; i++)
        {
            Population.Add((IPolygonGenesContainer)container.Clone());
            Population[i].PenaltyEvaluation();
        }

        for (int i = 0; i <= size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Population[j].PenaltyEvaluation();
                Console.WriteLine(j + " - " + Population[j].Penalty);
                if (j < fuckmyass.Count)
                    if (fuckmyass[j] != Population[j].Penalty)
                        Console.WriteLine("FUCK!!!!!!");
            }
            if (i == size)
                break;
            Console.WriteLine("\n");
            Population[i].Randomize();
            Population[i].PenaltyEvaluation();
            fuckmyass.Add(Population[i].Penalty);
        }

        fuckmyass.Clear();
        for (int j = 0; j < size; j++)
        {
            Population[j].PenaltyEvaluation();
            fuckmyass.Add(Population[j].Penalty);
            Console.WriteLine(j + " - " + Population[j].Penalty);
        }

        for (int i = 0; i < 500; i++)
        {
            Console.WriteLine("iteration for evaluation: " + i);
            for (int j = 0; j < size; j++)
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
        IPolygonGenesContainer bestRoom = (IPolygonGenesContainer)Population[0].Clone();
        double lowestPenalty = Population[0].Penalty;
        int count = 0;
        while (KeepUp)
        {
            Population = Population.OrderBy(container => container.Penalty).ToList();
            if (lowestPenalty > Population[0].Penalty)
            {
                lowestPenalty = Population[0].Penalty;
                bestRoom = (IPolygonGenesContainer)Population[0].Clone();
                bestRoom.Penalty = lowestPenalty;
            }

            if (lowestPenalty <= 1)
                break;

            if (count % 500 == 0)
            {
                SerializeElement(0);

                Console.WriteLine(count + "\nPenalty: " + Population[0].Penalty + "\nLowest penalty yet: " + lowestPenalty + "\n");
            }

            if (count % 10000 == 0)
            {
                Console.Write("Continue?\n\n1 - Yes;\nAny other symbol - No.\n\nEnter corresponding number: ");
                string? choice = Console.ReadLine();
                if (choice != null)
                {
                    if (choice != "1")
                        break;
                }
            }

            int transfer = (70 * Population.Count) / 100;

            List<IPolygonGenesContainer> newContainersSet = SUS(transfer, Population, FromPenaltyToFitness());


            transfer = Population.Count - transfer;

            List<int> usedParentsOne = new();
            int parentsAmount = newContainersSet.Count;
            for (int i = 0; i < transfer; i++)
            {
                int indexParent1;
                while (true)
                {
                    indexParent1 = new Random().Next(parentsAmount);
                    if (usedParentsOne.Contains(indexParent1))
                        continue;
                    break;
                }
                usedParentsOne.Add(indexParent1);
                int indexParent2;
                while (true)
                {
                    indexParent2 = new Random().Next(parentsAmount);
                    if (indexParent1 != indexParent2 && !usedParentsOne.Contains(indexParent2))
                        break;
                }
                newContainersSet.Add(newContainersSet[indexParent1].Crossover(newContainersSet[indexParent2]));
            }

            for (int i = 0; i < newContainersSet.Count; i++)
            {
                newContainersSet[i].PenaltyEvaluation();
            }

            var newPoluation = MutatePopulations(newContainersSet);

            Population = newPoluation;
            for (int i = 0; i < Population.Count; i++)
            {
                Population[i].PenaltyEvaluation();
            }
            count++;
        }

        Population[0] = bestRoom;
        SerializeElement(0);
        return 1;
    }

    public List<double> FromPenaltyToFitness()
    {
        double maxPenalty = Population.Last().Penalty;

        List<double> fitnessOfPopulation = new();

        foreach (var population in Population)
        {
            fitnessOfPopulation.Add(population.Penalty);
        }
        fitnessOfPopulation.Reverse();

        return fitnessOfPopulation;
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
        int amountToMutate = newContainersSet.Count / 2;
        if (amountToMutate == 0) amountToMutate = 1;

        List<IPolygonGenesContainer> mutated = new();

        while (true)
        {
            for (int i = 0; i < newContainersSet.Count; i++)
            {
                int chance = new Random().Next(100);

                if (chance > 49 && !mutated.Contains(newContainersSet[i]))
                {
                    MutatePopulation(newContainersSet[i]);
                    mutated.Add(newContainersSet[i]);
                }

                if (mutated.Count == amountToMutate) { return newContainersSet; }
            }
        }
    }

    private void MutatePopulation(IPolygonGenesContainer container)
    {
        container.Mutate();
    }

    private static List<IPolygonGenesContainer> SUS(int amountToKeep, List<IPolygonGenesContainer> containers, List<double> fitness)
    {
        Random a = new();

        double totalPenalty = 0;
        for (int i = 0; i < containers.Count; i++)
            totalPenalty += fitness[i];

        int distance = (int)(totalPenalty / amountToKeep);

        List<int> pointers = new();
        for (int i = 0; i < amountToKeep; i++)
            pointers.Add(a.Next(distance) + i * distance);

        return RWS(containers, pointers, fitness);
    }

    private static List<IPolygonGenesContainer> RWS(List<IPolygonGenesContainer> containers, List<int> pointers, List<double> fitness)
    {
        List<IPolygonGenesContainer> containersToKeep = new();
        //pointers.Reverse();
        List<int> selectorsUsed = new();
        for (int i = 0; i < pointers.Count; i++)
        {
            int selector = -1;
            double sum = 0;

            while (sum < pointers[i])
            {
                selector++;
                sum += fitness[i];
            }

            while (selector < 0)
                selector++;
            while (selector >= containers.Count)
                selector--;

            int tempSelector = selector;
            bool check = true;
            while (containersToKeep.Contains(containers[tempSelector]))
            {
                check = false;

                if (tempSelector == 0)
                    break;
                tempSelector--;

                check = true;
            }
            if (check)
            {
                containersToKeep.Add(containers[tempSelector]);
                selectorsUsed.Add(tempSelector);

                continue;
            }

            tempSelector = selector;
            check = true;
            while (containersToKeep.Contains(containers[tempSelector]))
            {
                check = false;

                if (tempSelector + 1 >= containers.Count)
                    break;
                tempSelector++;

                check = true;
            }
            if (check)
            {
                containersToKeep.Add(containers[tempSelector]);
                selectorsUsed.Add(tempSelector);

                continue;
            }
        }
        //selectorsUsed = selectorsUsed.OrderDescending().ToList();
        return containersToKeep;
    }
}
