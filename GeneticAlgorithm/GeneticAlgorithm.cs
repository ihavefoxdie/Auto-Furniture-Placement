using Interfaces;
using System.Drawing;
using System.Text.Json;

namespace GeneticAlgorithm;

public class GeneticAlgoritm
{
    public List<IPolygonGenesContainer> Population { get; private set; }
    private bool KeepUp { get; set; }
    public int Generation { get; private set; }


    public GeneticAlgoritm(IPolygonGenesContainer container)
    {
        Population = new();
        KeepUp = true;
        int size = 8;
        for (int i = 0; i < size; i++)
        {
            Population.Add((IPolygonGenesContainer)container.Clone());
        }
        Nuke();
    }

    private void Nuke()
    {
        Task[] tasks = new Task[Population.Count];
        for (int i = 0; i < Population.Count; i++)
        {
            var item = Population[i];
            tasks[i] = new Task(() =>
            {
                item.Randomize();
                item.PenaltyEvaluation();
                Console.WriteLine(item.Penalty);
            });
            tasks[i].Start();
        }
        Task.WaitAll(tasks);
    }

    public int Start()
    {
        IPolygonGenesContainer bestRoom = (IPolygonGenesContainer)Population[0].Clone();
        double lowestPenalty = Population[0].Penalty;
        int count = 0;
        int penaltyStagnationCount = 0;
        double mutationModifier = 1;
        int amountToMutate = Population.Count / 2;
        while (KeepUp)
        {
            Population = Population.OrderBy(container => container.Penalty).ToList();
            if (lowestPenalty > Population[0].Penalty)
            {
                lowestPenalty = Population[0].Penalty;
                bestRoom = (IPolygonGenesContainer)Population[0].Clone();
                bestRoom.Penalty = lowestPenalty;
                SerializeElement(0);
                penaltyStagnationCount = 0;
                mutationModifier = 1;
                amountToMutate = Population.Count / 2;
            }
            penaltyStagnationCount++;

            if(penaltyStagnationCount % 250 == 0)
            {
                mutationModifier += 0.25;
            }
            
            if(penaltyStagnationCount % 1000 == 0)
            {
                amountToMutate = Population.Count / 4;
            }

            if (penaltyStagnationCount % 4000 == 0)
            {
                Population[0] = (IPolygonGenesContainer)bestRoom.Clone();
            }

            if (penaltyStagnationCount % 6000 == 0)
            {
                Console.WriteLine("Population is being nuked for prolonged stagnation!");

                Nuke();

                penaltyStagnationCount = 0;
                mutationModifier = 1;
                amountToMutate = Population.Count / 2;
            }

            if (lowestPenalty <= 0)
                break;

            if (count % 1000 == 0)
            {
                Console.WriteLine(count + "\nPenalty: " + Population[0].Penalty + "\nLowest penalty yet: " + lowestPenalty + "\n");
            }


            if (count % 10000 == 0)
            {
                if(penaltyStagnationCount % 6000 == 0)
                {
                    Console.WriteLine("Evolution is too stagnant, the next cycle will start with the randomization of the population (basically starting over)!");
                }    
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

            List<int> usedParents = new();
            int parentsAmount = newContainersSet.Count;
            for (int i = 0; i < transfer; i++)
            {
                int indexParent1;
                while (true)
                {
                    indexParent1 = new Random().Next(parentsAmount);
                    if (usedParents.Contains(indexParent1))
                        continue;
                    break;
                }
                usedParents.Add(indexParent1);
                int indexParent2;
                while (true)
                {
                    indexParent2 = new Random().Next(parentsAmount);
                    if (indexParent1 != indexParent2 && !usedParents.Contains(indexParent2))
                        break;
                }
                newContainersSet.Add(newContainersSet[indexParent1].Crossover(newContainersSet[indexParent2]));
            }

            for (int i = 0; i < newContainersSet.Count; i++)
            {
                newContainersSet[i].PenaltyEvaluation();
            }

            var newPoluation = MutatePopulations(newContainersSet, newContainersSet.Count / 2, mutationModifier);

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

    private static List<IPolygonGenesContainer> MutatePopulations(List<IPolygonGenesContainer> newContainersSet, int amountToMutate, double mutationModifier = 1)
    {
        Task[] tasks = new Task[amountToMutate];
        int index = 0;
        List<IPolygonGenesContainer> mutatedItems = new();

        while (mutatedItems.Count < amountToMutate)
        {
            for (int i = 0; i < newContainersSet.Count; i++)
            {
                if (mutatedItems.Count == amountToMutate)
                    break;
                if (new Random().Next(0, 100) > 49 && !mutatedItems.Contains(newContainersSet[i]))
                {
                    mutatedItems.Add(newContainersSet[i]);
                    var item = newContainersSet[i];
                    tasks[index] = new Task(() => item.Mutate(mutationModifier));
                    tasks[index].Start();
                    index++;
                }
            }
        }
        Task.WaitAll(tasks);
        return newContainersSet;
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
