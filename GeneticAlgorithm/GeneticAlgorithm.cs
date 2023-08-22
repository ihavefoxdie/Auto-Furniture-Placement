using Interfaces;
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
        for (int i = 0; i < 6; i++)
        {
            Population.Add(container);
        }
    }
    //TODO CROSSOVER AND MUTATION ALGORITHMS
    public IPolygonGenesContainer? Start()
    {
        for (int i = 0; i < Population.Count; i++)
        {
            Population[i].PenaltyEvaluation();
        }
        while (KeepUp)
        {
            Population = Population.OrderBy(container => container.Penalty).ToList();

            int transfer = (70 * Population.Count) / 100;

            List<IPolygonGenesContainer> newContainersSet = SUS(transfer, Population);

            transfer = Population.Count - transfer;

            for (int i = 0; i < transfer; i++)
            {
                int indexParent1 = new Random().Next(Population.Count);
                int indexParent2;
                while (true)
                {
                    indexParent2 = new Random().Next(Population.Count);
                    if (indexParent1 != indexParent2)
                        break;
                }
                newContainersSet.Add(Population[indexParent1].Crossover(Population[indexParent2]));
            }

            int evenOrNot = new Random().Next(2);
            for (int i = 0; i < newContainersSet.Count; i++)
            {
                if (i % 2 == evenOrNot)
                {
                    newContainersSet[i].Mutate();
                }
            }
            Population = newContainersSet;
            for (int i = 0; i < Population.Count; i++)
            {
                Population[i].PenaltyEvaluation();
            }
        }
        return null;
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
            while (sum < pointers[i])
            {
                selector++;
                sum += containers[selector].Penalty;
            }
            containersToKeep.Add(containers[selector]);
        }

        return containersToKeep;
    }
}
